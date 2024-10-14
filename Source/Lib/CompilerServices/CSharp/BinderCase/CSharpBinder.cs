using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.CSharp.Facts;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;

namespace Luthetus.CompilerServices.CSharp.BinderCase;

public partial class CSharpBinder : IBinder
{
	private readonly Dictionary<ResourceUri, IBinderSession> _binderSessionMap = new();
	//private readonly object _binderSessionMapLock = new();
	
	/// <summary>
    /// The key for _symbolDefinitions is calculated by <see cref="ISymbol.GetSymbolDefinitionId"/>
    /// </summary>
	private readonly Dictionary<string, SymbolDefinition> _symbolDefinitions = new();
    private readonly Dictionary<string, NamespaceGroupNode> _namespaceGroupNodeMap = CSharpFacts.Namespaces.GetInitialBoundNamespaceStatementNodes();
    /// <summary>
    /// All of the type definitions should be maintainted in this dictionary as they are
    /// found via parsing. Then, when one types an ambiguous identifier, perhaps they
    /// wanted a type, and a lookup in this map can be done, and a using statement
    /// inserted for the user if they decide to use that autocomplete option.
    /// </summary>
    private readonly Dictionary<NamespaceAndTypeIdentifiers, TypeDefinitionNode> _allTypeDefinitions = new();
    private readonly IScope _globalScope = CSharpFacts.ScopeFacts.GetInitialGlobalScope();
    private readonly NamespaceStatementNode _topLevelNamespaceStatementNode = CSharpFacts.Namespaces.GetTopLevelNamespaceStatementNode();
    
    public CSharpBinder()
    {
    	var globalBinderSession = StartBinderSession(ResourceUri.Empty);
    	globalBinderSession.ScopeList.Add(_globalScope);
    	FinalizeBinderSession(globalBinderSession);
        // _boundScopes.Add(_globalScope.ResourceUri, new List<IScope> { _globalScope });
    }

    public ImmutableDictionary<string, NamespaceGroupNode> NamespaceGroupNodes => _namespaceGroupNodeMap.ToImmutableDictionary();
    public ImmutableArray<ISymbol> Symbols => _symbolDefinitions.Values.SelectMany(x => x.SymbolReferences).Select(x => x.Symbol).ToImmutableArray();
    public Dictionary<string, SymbolDefinition> SymbolDefinitions => _symbolDefinitions;
    public ImmutableDictionary<NamespaceAndTypeIdentifiers, TypeDefinitionNode> AllTypeDefinitions => _allTypeDefinitions.ToImmutableDictionary();
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList => ImmutableArray<TextEditorDiagnostic>.Empty;

    ImmutableArray<ITextEditorSymbol> IBinder.SymbolsList => Symbols
        .Select(s => (ITextEditorSymbol)s)
        .ToImmutableArray();

	/// <summary><see cref="FinalizeBinderSession"/></summary>
    public IBinderSession StartBinderSession(ResourceUri resourceUri)
    {
    	foreach (var namespaceGroupNodeKvp in _namespaceGroupNodeMap)
        {
            var keepStatements = namespaceGroupNodeKvp.Value.NamespaceStatementNodeList
                .Where(x => x.IdentifierToken.TextSpan.ResourceUri != resourceUri)
                .ToImmutableArray();

            _namespaceGroupNodeMap[namespaceGroupNodeKvp.Key] =
                new NamespaceGroupNode(
                    namespaceGroupNodeKvp.Value.NamespaceString,
                    keepStatements);
        }

        foreach (var symbolDefinition in _symbolDefinitions)
        {
            var keep = symbolDefinition.Value.SymbolReferences
                .Where(x => x.Symbol.TextSpan.ResourceUri != resourceUri)
                .ToList();

            _symbolDefinitions[symbolDefinition.Key] =
                symbolDefinition.Value with
                {
                    SymbolReferences = keep
                };
        }
    
        return new CSharpBinderSession(
            resourceUri,
            this,
            _globalScope.Key,
            _topLevelNamespaceStatementNode);
    }

	/// <summary><see cref="StartBinderSession"/></summary>
	public void FinalizeBinderSession(IBinderSession binderSession)
	{
		UpsertBinderSession(binderSession);
	}

    public LiteralExpressionNode BindLiteralExpressionNode(
        LiteralExpressionNode literalExpressionNode,
        CSharpParserModel model)
    {
    	TypeClauseNode typeClauseNode;
    
    	switch (literalExpressionNode.LiteralSyntaxToken.SyntaxKind)
    	{
    		case SyntaxKind.NumericLiteralToken:
    			typeClauseNode = CSharpFacts.Types.Int.ToTypeClause();
    			break;
            case SyntaxKind.CharLiteralToken:
            	typeClauseNode = CSharpFacts.Types.Char.ToTypeClause();
            	break;
            case SyntaxKind.StringLiteralToken:
            	typeClauseNode = CSharpFacts.Types.String.ToTypeClause();
            	break;
            default:
            	typeClauseNode = CSharpFacts.Types.Void.ToTypeClause();
            	model.DiagnosticBag.ReportTodoException(literalExpressionNode.LiteralSyntaxToken.TextSpan, $"{nameof(BindLiteralExpressionNode)}(...) failed to map SyntaxKind: '{literalExpressionNode.LiteralSyntaxToken.SyntaxKind}'");
            	break;
    	}

        return new LiteralExpressionNode(
            literalExpressionNode.LiteralSyntaxToken,
            typeClauseNode);
    }

    public BinaryOperatorNode BindBinaryOperatorNode(
        IExpressionNode leftExpressionNode,
        ISyntaxToken operatorToken,
        IExpressionNode rightExpressionNode,
        CSharpParserModel parserModel)
    {
        var problematicTextSpan = (TextEditorTextSpan?)null;

        if (leftExpressionNode.ResultTypeClauseNode.ValueType == typeof(int))
        {
            if (rightExpressionNode.ResultTypeClauseNode.ValueType == typeof(int))
            {
                switch (operatorToken.SyntaxKind)
                {
                    case SyntaxKind.PlusToken:
                    case SyntaxKind.MinusToken:
                    case SyntaxKind.StarToken:
                    case SyntaxKind.DivisionToken:
                        return new BinaryOperatorNode(
                            leftExpressionNode.ResultTypeClauseNode,
                            operatorToken,
                            rightExpressionNode.ResultTypeClauseNode,
                            CSharpFacts.Types.Int.ToTypeClause());
                }
            }
            else
            {
                problematicTextSpan = rightExpressionNode.ConstructTextSpanRecursively();
            }
        }
        else if (leftExpressionNode.ResultTypeClauseNode.ValueType == typeof(string))
        {
            if (rightExpressionNode.ResultTypeClauseNode.ValueType == typeof(string))
            {
                switch (operatorToken.SyntaxKind)
                {
                    case SyntaxKind.PlusToken:
                        return new BinaryOperatorNode(
                            leftExpressionNode.ResultTypeClauseNode,
                            operatorToken,
                            rightExpressionNode.ResultTypeClauseNode,
                            CSharpFacts.Types.String.ToTypeClause());
                }
            }
            else
            {
                problematicTextSpan = rightExpressionNode.ConstructTextSpanRecursively();
            }
        }
        else
        {
            problematicTextSpan = leftExpressionNode.ConstructTextSpanRecursively();
        }

        if (problematicTextSpan is not null)
        {
            var errorMessage = $"Operator: {operatorToken.TextSpan.GetText()} is not defined" +
                $" for types: {leftExpressionNode.ConstructTextSpanRecursively().GetText()}" +
                $" and {rightExpressionNode.ConstructTextSpanRecursively().GetText()}";

            parserModel.DiagnosticBag.ReportTodoException(problematicTextSpan.Value, errorMessage);
        }

        return new BinaryOperatorNode(
            leftExpressionNode.ResultTypeClauseNode,
            operatorToken,
            rightExpressionNode.ResultTypeClauseNode,
            CSharpFacts.Types.Void.ToTypeClause());
    }

    /// <summary>TODO: Construct a BoundStringInterpolationExpressionNode and identify the expressions within the string literal. For now I am just making the dollar sign the same color as a string literal.</summary>
    public void BindStringInterpolationExpression(
        DollarSignToken dollarSignToken,
        CSharpParserModel model)
    {
        AddSymbolReference(new StringInterpolationSymbol(dollarSignToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.StringLiteral,
        }), model);
    }
    
    public void BindStringVerbatimExpression(
        AtToken atToken,
        CSharpParserModel model)
    {
        AddSymbolReference(new StringVerbatimSymbol(atToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.StringLiteral,
        }), model);
    }

    public void BindFunctionDefinitionNode(
        FunctionDefinitionNode functionDefinitionNode,
        CSharpParserModel model)
    {
        var functionIdentifierText = functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText();

        var functionSymbol = new FunctionSymbol(functionDefinitionNode.FunctionIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Function
        });

        AddSymbolDefinition(functionSymbol, model);

        if (!TryAddFunctionDefinitionNodeByScope(
        		model,
        		model.BinderSession.ResourceUri,
        		model.BinderSession.CurrentScopeKey,
        		functionIdentifierText,
                functionDefinitionNode))
        {
            model.BinderSession.DiagnosticBag.ReportAlreadyDefinedFunction(
                functionDefinitionNode.FunctionIdentifierToken.TextSpan,
                functionIdentifierText);
        }
    }
    
    void IBinder.BindFunctionOptionalArgument(FunctionArgumentEntryNode functionArgumentEntryNode, IParserModel model) =>
    	BindFunctionOptionalArgument(functionArgumentEntryNode, (CSharpParserModel)model);

    public void BindFunctionOptionalArgument(
        FunctionArgumentEntryNode functionArgumentEntryNode,
        CSharpParserModel model)
    {
        var argumentTypeClauseNode = functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode;

        if (TryGetTypeDefinitionHierarchically(
        		model,
        		model.BinderSession.ResourceUri,
                model.BinderSession.CurrentScopeKey,
                argumentTypeClauseNode.TypeIdentifierToken.TextSpan.GetText(),
                out var typeDefinitionNode)
            || typeDefinitionNode is null)
        {
            typeDefinitionNode = CSharpFacts.Types.Void;
        }

        var literalExpressionNode = new LiteralExpressionNode(
            functionArgumentEntryNode.OptionalCompileTimeConstantToken,
            typeDefinitionNode.ToTypeClause());

        literalExpressionNode = BindLiteralExpressionNode(literalExpressionNode, model);

        if (literalExpressionNode.ResultTypeClauseNode.ValueType is null ||
            literalExpressionNode.ResultTypeClauseNode.ValueType != functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode.ValueType)
        {
            var optionalArgumentTextSpan = functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode.TypeIdentifierToken.TextSpan with
            {
                EndingIndexExclusive = functionArgumentEntryNode.VariableDeclarationNode.IdentifierToken.TextSpan.EndingIndexExclusive
            };

            model.BinderSession.DiagnosticBag.ReportBadFunctionOptionalArgumentDueToMismatchInType(
                optionalArgumentTextSpan,
                functionArgumentEntryNode.VariableDeclarationNode.IdentifierToken.TextSpan.GetText(),
                functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode.ValueType?.Name ?? "null",
                literalExpressionNode.ResultTypeClauseNode.ValueType?.Name ?? "null");
        }
    }

    /// <summary>TODO: Validate that the returned bound expression node has the same result type as the enclosing scope.</summary>
    public ReturnStatementNode BindReturnStatementNode(
        KeywordToken keywordToken,
        IExpressionNode expressionNode)
    {
        return new ReturnStatementNode(
            keywordToken,
            expressionNode);
    }

    public IfStatementNode BindIfStatementNode(
        KeywordToken ifKeywordToken,
        IExpressionNode expressionNode)
    {
        var boundIfStatementNode = new IfStatementNode(
            ifKeywordToken,
            expressionNode,
            null);

        return boundIfStatementNode;
    }

    public void SetCurrentNamespaceStatementNode(
        NamespaceStatementNode namespaceStatementNode,
        CSharpParserModel model)
    {
        model.BinderSession.CurrentNamespaceStatementNode = namespaceStatementNode;
    }

    public void BindNamespaceStatementNode(
        NamespaceStatementNode namespaceStatementNode,
        CSharpParserModel model)
    {
        var namespaceString = namespaceStatementNode.IdentifierToken.TextSpan.GetText();
        AddSymbolReference(new NamespaceSymbol(namespaceStatementNode.IdentifierToken.TextSpan), model);

        if (_namespaceGroupNodeMap.TryGetValue(namespaceString, out var inNamespaceGroupNode))
        {
            var outNamespaceStatementNodeList = inNamespaceGroupNode.NamespaceStatementNodeList
                .Add(namespaceStatementNode)
                .ToImmutableArray();

            var outNamespaceGroupNode = new NamespaceGroupNode(
                inNamespaceGroupNode.NamespaceString,
                outNamespaceStatementNodeList);

            _namespaceGroupNodeMap[namespaceString] = outNamespaceGroupNode;
        }
        else
        {
            _namespaceGroupNodeMap.Add(namespaceString, new NamespaceGroupNode(
                namespaceString,
                new NamespaceStatementNode[] { namespaceStatementNode }.ToImmutableArray()));
        }
    }

    public void BindConstructorInvocationNode()
    {
        // Deleted what was in this method because it was nonsense, and causing errors. (2023-08-06)
    }

    public InheritanceStatementNode BindInheritanceStatementNode(
        TypeClauseNode typeClauseNode,
        CSharpParserModel model)
    {
        AddSymbolReference(new TypeSymbol(typeClauseNode.TypeIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type
        }), model);

        model.DiagnosticBag.ReportTodoException(
            typeClauseNode.TypeIdentifierToken.TextSpan,
            $"Implement {nameof(BindInheritanceStatementNode)}");

        return new InheritanceStatementNode(typeClauseNode);
    }

	void IBinder.BindVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode, IParserModel model) =>
		BindVariableDeclarationNode(variableDeclarationNode, (CSharpParserModel)model);

    public void BindVariableDeclarationNode(
        IVariableDeclarationNode variableDeclarationNode,
        CSharpParserModel model)
    {
        CreateVariableSymbol(variableDeclarationNode.IdentifierToken, variableDeclarationNode.VariableKind, model);
        var text = variableDeclarationNode.IdentifierToken.TextSpan.GetText();
        
        if (TryGetVariableDeclarationNodeByScope(
        		model,
        		model.BinderSession.ResourceUri,
        		model.BinderSession.CurrentScopeKey,
        		text,
        		out var existingVariableDeclarationNode))
        {
            if (existingVariableDeclarationNode.IsFabricated)
            {
                // Overwrite the fabricated definition with a real one
                //
                // TODO: Track one or many declarations?...
                // (if there is an error where something is defined twice for example)
                SetVariableDeclarationNodeByScope(
        		model,
                	model.BinderSession.ResourceUri,
        			model.BinderSession.CurrentScopeKey,
                	text,
                	variableDeclarationNode);
            }

            model.BinderSession.DiagnosticBag.ReportAlreadyDefinedVariable(
                variableDeclarationNode.IdentifierToken.TextSpan,
                text);
        }
        else
        {
        	_ = TryAddVariableDeclarationNodeByScope(
        		model,
        		model.BinderSession.ResourceUri,
    			model.BinderSession.CurrentScopeKey,
            	text,
            	variableDeclarationNode);
        }
    }

    public VariableReferenceNode ConstructAndBindVariableReferenceNode(
        IdentifierToken variableIdentifierToken,
        CSharpParserModel model)
    {
        var text = variableIdentifierToken.TextSpan.GetText();
        VariableReferenceNode? variableReferenceNode;

        if (TryGetVariableDeclarationHierarchically(
        		model,
                model.BinderSession.ResourceUri,
                model.BinderSession.CurrentScopeKey,
                text,
                out var variableDeclarationNode)
            && variableDeclarationNode is not null)
        {
            variableReferenceNode = new VariableReferenceNode(
                variableIdentifierToken,
                variableDeclarationNode);
        }
        else
        {
            variableDeclarationNode = new VariableDeclarationNode(
                CSharpFacts.Types.Var.ToTypeClause(),
                variableIdentifierToken,
                VariableKind.Local,
                false)
            {
                IsFabricated = true,
            };

            variableReferenceNode = new VariableReferenceNode(
                variableIdentifierToken,
                variableDeclarationNode);

            model.BinderSession.DiagnosticBag.ReportUndefinedVariable(
                variableIdentifierToken.TextSpan,
                text);
        }

        CreateVariableSymbol(variableReferenceNode.VariableIdentifierToken, variableDeclarationNode.VariableKind, model);
        return variableReferenceNode;
    }

    public void BindVariableAssignmentExpressionNode(
        VariableAssignmentExpressionNode variableAssignmentExpressionNode,
        CSharpParserModel model)
    {
        var text = variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan.GetText();
        VariableKind variableKind = VariableKind.Local;

        if (TryGetVariableDeclarationHierarchically(
        		model,
                model.BinderSession.ResourceUri,
                model.BinderSession.CurrentScopeKey,
                text,
                out var variableDeclarationNode)
            && variableDeclarationNode is not null)
        {
            variableKind = variableDeclarationNode.VariableKind;

            // TODO: Remove the setter from 'VariableDeclarationNode'...
            // ...and set IsInitialized to true by overwriting the VariableDeclarationMap.
            variableDeclarationNode.IsInitialized = true;
        }
        else
        {
            if (UtilityApi.IsContextualKeywordSyntaxKind(text))
            {
                model.BinderSession.DiagnosticBag.TheNameDoesNotExistInTheCurrentContext(
                    variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan,
                    text);
            }
            else
            {
                model.BinderSession.DiagnosticBag.ReportUndefinedVariable(
                    variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan,
                    text);
            }
        }

        CreateVariableSymbol(variableAssignmentExpressionNode.VariableIdentifierToken, variableKind, model);
    }

    public void BindConstructorDefinitionIdentifierToken(
        IdentifierToken identifierToken,
        CSharpParserModel model)
    {
        var constructorSymbol = new ConstructorSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type
        });

        AddSymbolDefinition(constructorSymbol, model);
    }

    public void BindFunctionInvocationNode(
        FunctionInvocationNode functionInvocationNode,
        CSharpParserModel model)
    {
        var functionInvocationIdentifierText = functionInvocationNode
            .FunctionInvocationIdentifierToken.TextSpan.GetText();

        var functionSymbol = new FunctionSymbol(functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Function
        });

        AddSymbolReference(functionSymbol, model);

        if (TryGetFunctionHierarchically(
        		model,
                model.BinderSession.ResourceUri,
                model.BinderSession.CurrentScopeKey,
                functionInvocationIdentifierText,
                out var functionDefinitionNode) &&
            functionDefinitionNode is not null)
        {
            return;
        }
        else
        {
            model.BinderSession.DiagnosticBag.ReportUndefinedFunction(
                functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan,
                functionInvocationIdentifierText);
        }
    }

    public void BindNamespaceReference(
        IdentifierToken namespaceIdentifierToken,
        CSharpParserModel model)
    {
        var namespaceSymbol = new NamespaceSymbol(namespaceIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.None
        });

        AddSymbolReference(namespaceSymbol, model);
    }

    public TypeClauseNode BindTypeClauseNode(
        TypeClauseNode typeClauseNode,
        CSharpParserModel model)
    {
        if (typeClauseNode.TypeIdentifierToken.SyntaxKind == SyntaxKind.IdentifierToken)
        {
            var typeSymbol = new TypeSymbol(typeClauseNode.TypeIdentifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            });

            AddSymbolReference(typeSymbol, model);
        }

        var matchingTypeDefintionNode = CSharpFacts.Types.TypeDefinitionNodes.SingleOrDefault(
            x => x.TypeIdentifierToken.TextSpan.GetText() == typeClauseNode.TypeIdentifierToken.TextSpan.GetText());

        if (matchingTypeDefintionNode is not null)
        {
            return new TypeClauseNode(
                typeClauseNode.TypeIdentifierToken,
                matchingTypeDefintionNode.ValueType,
                typeClauseNode.GenericParametersListingNode);
        }

        return typeClauseNode;
    }

    public void BindTypeIdentifier(
        IdentifierToken identifierToken,
        CSharpParserModel model)
    {
        if (identifierToken.SyntaxKind == SyntaxKind.IdentifierToken)
        {
            var typeSymbol = new TypeSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            });

            AddSymbolReference(typeSymbol, model);
        }
    }

    public UsingStatementNode BindUsingStatementNode(
        KeywordToken usingKeywordToken,
        IdentifierToken namespaceIdentifierToken,
        CSharpParserModel model)
    {
        AddSymbolReference(new NamespaceSymbol(namespaceIdentifierToken.TextSpan), model);

        var usingStatementNode = new UsingStatementNode(
            usingKeywordToken,
            namespaceIdentifierToken);

        model.BinderSession.CurrentUsingStatementNodeList.Add(usingStatementNode);
        AddNamespaceToCurrentScope(namespaceIdentifierToken.TextSpan.GetText(), model);

        return usingStatementNode;
    }

    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
    public AttributeNode BindAttributeNode(
        OpenSquareBracketToken openSquareBracketToken,
        List<ISyntaxToken> innerTokens,
        CloseSquareBracketToken closeSquareBracketToken,
        CSharpParserModel model)
    {
        AddSymbolReference(new TypeSymbol(openSquareBracketToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type,
            EndingIndexExclusive = closeSquareBracketToken.TextSpan.EndingIndexExclusive
        }), model);

        return new AttributeNode(
            openSquareBracketToken,
            innerTokens,
            closeSquareBracketToken);
    }

    public void RegisterScope(
        TypeClauseNode? scopeReturnTypeClauseNode,
        TextEditorTextSpan textSpan,
        CSharpParserModel model)
    {
        var scope = new Scope(
        	key: Key<IScope>.NewKey(),
		    parentKey: model.BinderSession.CurrentScopeKey,
		    model.BinderSession.ResourceUri,
		    textSpan.StartingIndexInclusive,
		    endingIndexExclusive: null);
		    
		var indexForInsertion = model.BinderSession.ScopeList.FindIndex(x =>
			scope.StartingIndexInclusive < x.StartingIndexInclusive);
			
		if (indexForInsertion == -1)
        	indexForInsertion = model.BinderSession.ScopeList.Count;
        	
        model.BinderSession.ScopeList.Insert(indexForInsertion, scope);
        model.BinderSession.CurrentScopeKey = scope.Key;
    }

	void IBinder.AddNamespaceToCurrentScope(string namespaceString, IParserModel model) =>
		AddNamespaceToCurrentScope(namespaceString, (CSharpParserModel)model);

    public void AddNamespaceToCurrentScope(
        string namespaceString,
        CSharpParserModel model)
    {
        if (_namespaceGroupNodeMap.TryGetValue(namespaceString, out var namespaceGroupNode) &&
            namespaceGroupNode is not null)
        {
            var typeDefinitionNodes = namespaceGroupNode.GetTopLevelTypeDefinitionNodes();

            foreach (var typeDefinitionNode in typeDefinitionNodes)
            {
            	_ = TryAddTypeDefinitionNodeByScope(
        				model,
	            		model.BinderSession.ResourceUri,
	            		model.BinderSession.CurrentScopeKey,
	            		typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText(),
	            		typeDefinitionNode);
            }
        }
    }

    public void DisposeScope(
        TextEditorTextSpan textSpan,
        CSharpParserModel model)
    {
    	// Check if it is the global scope, if so return early.
    	if (model.BinderSession.CurrentScopeKey == Key<IScope>.Empty)
    		return;
    	
    	// It is a struct, but needs to be mutated;
    	var indexOf = model.BinderSession.ScopeList.FindIndex(x => x.Key == model.BinderSession.CurrentScopeKey);
    	if (indexOf == -1)
    		return;
    	
    	// "mutate"
    	var scope = model.BinderSession.ScopeList[indexOf];
    	scope.EndingIndexExclusive = textSpan.EndingIndexExclusive;
    	model.BinderSession.ScopeList[indexOf] = scope;

        if (scope.ParentKey is not null)
            model.BinderSession.CurrentScopeKey = scope.ParentKey.Value;
    }

    public void BindTypeDefinitionNode(
        TypeDefinitionNode typeDefinitionNode,
        CSharpParserModel model,
        bool shouldOverwrite = false)
    {
        var typeIdentifierText = typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText();
        var currentNamespaceStatementText = model.BinderSession.CurrentNamespaceStatementNode.IdentifierToken.TextSpan.GetText();
        var namespaceAndTypeIdentifiers = new NamespaceAndTypeIdentifiers(currentNamespaceStatementText, typeIdentifierText);

        typeDefinitionNode.EncompassingNamespaceIdentifierString = currentNamespaceStatementText;
        
        if (TryGetTypeDefinitionNodeByScope(
        		model,
        		model.BinderSession.ResourceUri,
        		model.BinderSession.CurrentScopeKey,
        		typeIdentifierText,
        		out var existingTypeDefinitionNode))
        {
        	if (shouldOverwrite || existingTypeDefinitionNode.IsFabricated)
        	{
        		SetTypeDefinitionNodeByScope(
        			model,
        			model.BinderSession.ResourceUri,
	        		model.BinderSession.CurrentScopeKey,
	        		typeIdentifierText,
	        		typeDefinitionNode);
        	}
        }
        else
        {
        	_ = TryAddTypeDefinitionNodeByScope(
        		model,
    			model.BinderSession.ResourceUri,
        		model.BinderSession.CurrentScopeKey,
        		typeIdentifierText,
        		typeDefinitionNode);
        }

        var success = _allTypeDefinitions.TryAdd(namespaceAndTypeIdentifiers, typeDefinitionNode);
        if (!success)
        {
        	var entryFromAllTypeDefinitions = _allTypeDefinitions[namespaceAndTypeIdentifiers];
        	
        	if (shouldOverwrite || entryFromAllTypeDefinitions.IsFabricated)
        		_allTypeDefinitions[namespaceAndTypeIdentifiers] = typeDefinitionNode;
        }
    }

    /// <summary>This method will handle the <see cref="SymbolDefinition"/>, but also invoke <see cref="AddSymbolReference"/> because each definition is being treated as a reference itself.</summary>
    private void AddSymbolDefinition(
        ISymbol symbol,
        CSharpParserModel model)
    {
        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            symbol.TextSpan.GetText(),
            model.BinderSession.CurrentScopeKey);

        var symbolDefinition = new SymbolDefinition(
            model.BinderSession.CurrentScopeKey,
            symbol);

        if (!_symbolDefinitions.TryAdd(
                symbolDefinitionId,
                symbolDefinition))
        {
            var existingSymbolDefinition = _symbolDefinitions[symbolDefinitionId];

            if (existingSymbolDefinition.IsFabricated)
            {
                _symbolDefinitions[symbolDefinitionId] = existingSymbolDefinition with
                {
                    IsFabricated = false
                };
            }
            // TODO: The else branch of this if statement would mean the Symbol definition was found twice, should a diagnostic be reported here?
        }

        AddSymbolReference(symbol, model);
    }

    private void AddSymbolReference(ISymbol symbol, CSharpParserModel model)
    {
        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            symbol.TextSpan.GetText(),
            model.BinderSession.CurrentScopeKey);

        if (!_symbolDefinitions.TryGetValue(
                symbolDefinitionId,
                out var symbolDefinition))
        {
            symbolDefinition = new SymbolDefinition(
                model.BinderSession.CurrentScopeKey,
                symbol)
            {
                IsFabricated = true
            };

            // TODO: Symbol definition was not found, should a diagnostic be reported here?
            var success = _symbolDefinitions.TryAdd(
                symbolDefinitionId,
                symbolDefinition);

            if (!success)
                _symbolDefinitions[symbolDefinitionId] = symbolDefinition;
        }

        symbolDefinition.SymbolReferences.Add(new SymbolReference(
            symbol,
            model.BinderSession.CurrentScopeKey));
    }

    public void CreateVariableSymbol(
        IdentifierToken identifierToken,
        VariableKind variableKind,
        CSharpParserModel model)
    {
        switch (variableKind)
        {
            case VariableKind.Field:
                AddSymbolDefinition(new FieldSymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Field
                }), model);
                break;
            case VariableKind.Property:
                AddSymbolDefinition(new PropertySymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Property
                }), model);
                break;
            case VariableKind.Local:
                goto default;
            case VariableKind.Closure:
                goto default;
            default:
                AddSymbolDefinition(new VariableSymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Variable
                }), model);
                break;
        }
    }

	/// <summary>
	/// Do not invoke this when re-parsing the same file.
	/// 
	/// Instead, only invoke this when the file is deleted,
	/// and should no longer be included in the binder.
	/// </summary>
    public void ClearStateByResourceUri(ResourceUri resourceUri)
    {
        foreach (var namespaceGroupNodeKvp in _namespaceGroupNodeMap)
        {
            var keepStatements = namespaceGroupNodeKvp.Value.NamespaceStatementNodeList
                .Where(x => x.IdentifierToken.TextSpan.ResourceUri != resourceUri)
                .ToImmutableArray();

            _namespaceGroupNodeMap[namespaceGroupNodeKvp.Key] =
                new NamespaceGroupNode(
                    namespaceGroupNodeKvp.Value.NamespaceString,
                    keepStatements);
        }

        foreach (var symbolDefinition in _symbolDefinitions)
        {
            var keep = symbolDefinition.Value.SymbolReferences
                .Where(x => x.Symbol.TextSpan.ResourceUri != resourceUri)
                .ToList();

            _symbolDefinitions[symbolDefinition.Key] =
                symbolDefinition.Value with
                {
                    SymbolReferences = keep
                };
        }

		_binderSessionMap.Remove(resourceUri);
    }
    
    /// <summary>
    /// Search hierarchically through all the scopes, starting at the <see cref="initialScope"/>.<br/><br/>
    /// If a match is found, then set the out parameter to it and return true.<br/><br/>
    /// If none of the searched scopes contained a match then set the out parameter to null and return false.
    /// </summary>
    public bool TryGetFunctionHierarchically(
    	IParserModel? parserModel,
        ResourceUri resourceUri,
    	Key<IScope> initialScopeKey,
        string identifierText,
        out FunctionDefinitionNode? functionDefinitionNode)
    {
        var localScope = GetScope(parserModel, resourceUri, initialScopeKey);

        while (localScope is not null)
        {
            if (TryGetFunctionDefinitionNodeByScope(
	        		parserModel,
            		resourceUri,
            		localScope.Key,
            		identifierText,
                    out functionDefinitionNode))
            {
                return true;
            }

			if (localScope.ParentKey is null)
				localScope = null;
			else
            	localScope = GetScope(parserModel, resourceUri, localScope.ParentKey.Value);
        }

        functionDefinitionNode = null;
        return false;
    }

    /// <summary>
    /// Search hierarchically through all the scopes, starting at the <see cref="initialScope"/>.<br/><br/>
    /// If a match is found, then set the out parameter to it and return true.<br/><br/>
    /// If none of the searched scopes contained a match then set the out parameter to null and return false.
    /// </summary>
    public bool TryGetTypeDefinitionHierarchically(
    	IParserModel? parserModel,
        ResourceUri resourceUri,
    	Key<IScope> initialScopeKey,
        string identifierText,
        out TypeDefinitionNode? typeDefinitionNode)
    {
        var localScope = GetScope(parserModel, resourceUri, initialScopeKey);

        while (localScope is not null)
        {
            if (TryGetTypeDefinitionNodeByScope(
	        		parserModel,
            		resourceUri,
            		localScope.Key,
            		identifierText,
                    out typeDefinitionNode))
            {
                return true;
            }

            if (localScope.ParentKey is null)
				localScope = null;
			else
            	localScope = GetScope(parserModel, resourceUri, localScope.ParentKey.Value);
        }

        typeDefinitionNode = null;
        return false;
    }

    /// <summary>
    /// Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>
    /// If a match is found, then set the out parameter to it and return true.<br/><br/>
    /// If none of the searched scopes contained a match then set the out parameter to null and return false.
    /// </summary>
    public bool TryGetVariableDeclarationHierarchically(
    	IParserModel? parserModel,
    	ResourceUri resourceUri,
    	Key<IScope> initialScopeKey,
        string identifierText,
        out IVariableDeclarationNode? variableDeclarationStatementNode)
    {
        var localScope = GetScope(parserModel, resourceUri, initialScopeKey);

        while (localScope is not null)
        {
            if (TryGetVariableDeclarationNodeByScope(
	        		parserModel,
            		resourceUri,
            		localScope.Key,
            		identifierText,
                    out variableDeclarationStatementNode))
            {
                return true;
            }

            if (localScope.ParentKey is null)
				localScope = null;
			else
            	localScope = GetScope(parserModel, resourceUri, localScope.ParentKey.Value);
        }

        variableDeclarationStatementNode = null;
        return false;
    }

    IScope? IBinder.GetScope(TextEditorTextSpan textSpan) =>
    	GetScope(model: null, textSpan);
    
    public IScope? GetScope(IParserModel? model, TextEditorTextSpan textSpan)
    {
    	return GetScope(model, textSpan.ResourceUri, textSpan.StartingIndexInclusive);
    }
    
    IScope? IBinder.GetScope(ResourceUri resourceUri, int positionIndex) =>
    	GetScope(model: null, resourceUri, positionIndex);
    
    public IScope? GetScope(IParserModel? model, ResourceUri resourceUri, int positionIndex)
    {
    	var scopeList = new List<IScope>();
    	
    	if (TryGetBinderSession(model, resourceUri, out var targetBinderSession))
    		scopeList.AddRange(targetBinderSession.ScopeList);
		if (TryGetBinderSession(model, ResourceUri.Empty, out var globalBinderSession))
    		scopeList.AddRange(globalBinderSession.ScopeList);
        
        var possibleScopes = scopeList.Where(x =>
        {
            return x.StartingIndexInclusive <= positionIndex &&
            	   // Global Scope awkwardly has a null ending index exclusive (2023-10-15)
                   (x.EndingIndexExclusive >= positionIndex || x.EndingIndexExclusive is null);
        });

        return possibleScopes.MinBy(x => positionIndex - x.StartingIndexInclusive);
    }
    
    IScope? IBinder.GetScope(ResourceUri resourceUri, Key<IScope> scopeKey) =>
    	GetScope(model: null, resourceUri, scopeKey);
    
    public IScope? GetScope(IParserModel? model, ResourceUri resourceUri, Key<IScope> scopeKey)
    {
    	var scopeList = new List<IScope>();
    	
    	if (TryGetBinderSession(model, resourceUri, out var targetBinderSession))
    		scopeList.AddRange(targetBinderSession.ScopeList);
		if (TryGetBinderSession(model, ResourceUri.Empty, out var globalBinderSession))
    		scopeList.AddRange(globalBinderSession.ScopeList);
        
        return scopeList.FirstOrDefault(x => x.Key == scopeKey);
    }
    
    IScope[]? IBinder.GetScopeList(ResourceUri resourceUri) =>
    	GetScopeList(model: null, resourceUri);
    
    public IScope[]? GetScopeList(IParserModel? model, ResourceUri resourceUri)
    {
    	var scopeList = new List<IScope>();
    
    	if (TryGetBinderSession(model, resourceUri, out var targetBinderSession))
    		scopeList.AddRange(targetBinderSession.ScopeList);
		if (TryGetBinderSession(model, ResourceUri.Empty, out var globalBinderSession))
    		scopeList.AddRange(globalBinderSession.ScopeList);
    		
    	return scopeList.ToArray();
    }
    
    bool IBinder.TryGetBinderSession(ResourceUri resourceUri, out IBinderSession binderSession) =>
    	TryGetBinderSession(parserModel: null, resourceUri, out binderSession);
    
    /// <summary>
    /// If the resourceUri is the in progress BinderSession's ResourceUri,
    /// then the in progress instance should be returned via the out variable.
    ///
    /// TODO: This is quite confusingly written at the moment. 
    /// </summary>
    public bool TryGetBinderSession(IParserModel? parserModel, ResourceUri resourceUri, out IBinderSession binderSession)
    {
    	if (parserModel is not null &&
    		resourceUri == parserModel.BinderSession.ResourceUri)
    	{
    		binderSession = parserModel.BinderSession;
    		return true;
    	}
    	
    	return _binderSessionMap.TryGetValue(resourceUri, out binderSession);
    }
    
    public void UpsertBinderSession(IBinderSession binderSession)
    {
    	try
    	{
    		if (_binderSessionMap.ContainsKey(binderSession.ResourceUri))
	    		_binderSessionMap[binderSession.ResourceUri] = binderSession;
	    	else
	    		_binderSessionMap.Add(binderSession.ResourceUri, binderSession);
    	}
    	catch (Exception e)
    	{
    		Console.WriteLine(e);
    	}
    }
    
    public bool RemoveBinderSession(ResourceUri resourceUri)
    {
    	return _binderSessionMap.Remove(resourceUri);
    }
    
    TypeDefinitionNode[] IBinder.GetTypeDefinitionNodesByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey) =>
	    GetTypeDefinitionNodesByScope(model: null, resourceUri, scopeKey);
    
    public TypeDefinitionNode[] GetTypeDefinitionNodesByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    		return Array.Empty<TypeDefinitionNode>();
    	
    	return binderSession.ScopeTypeDefinitionMap
    		.Where(kvp => kvp.Key.ScopeKey == scopeKey)
    		.Select(kvp => kvp.Value)
    		.ToArray();
    }
    
    bool IBinder.TryGetTypeDefinitionNodeByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey,
	    	string typeIdentifierText,
	    	out TypeDefinitionNode typeDefinitionNode) =>
	    TryGetTypeDefinitionNodeByScope(model: null, resourceUri, scopeKey, typeIdentifierText, out typeDefinitionNode);
    
    public bool TryGetTypeDefinitionNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
    	out TypeDefinitionNode typeDefinitionNode)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    	{
    		typeDefinitionNode = null;
    		return false;
    	}
    	
    	var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, typeIdentifierText);
    	return binderSession.ScopeTypeDefinitionMap.TryGetValue(scopeKeyAndIdentifierText, out typeDefinitionNode);
    }
    
    bool IBinder.TryAddTypeDefinitionNodeByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey,
	    	string typeIdentifierText,
	        TypeDefinitionNode typeDefinitionNode) =>
	    TryAddTypeDefinitionNodeByScope(model: null, resourceUri, scopeKey, typeIdentifierText, typeDefinitionNode);
    
    public bool TryAddTypeDefinitionNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    		return false;
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, typeIdentifierText);
    	return binderSession.ScopeTypeDefinitionMap.TryAdd(scopeKeyAndIdentifierText, typeDefinitionNode);
    }
    
    void IBinder.SetTypeDefinitionNodeByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey,
	    	string typeIdentifierText,
	        TypeDefinitionNode typeDefinitionNode) =>
	    SetTypeDefinitionNodeByScope(model: null, resourceUri, scopeKey, typeIdentifierText, typeDefinitionNode);
    
    public void SetTypeDefinitionNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    		return;

		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, typeIdentifierText);
    	binderSession.ScopeTypeDefinitionMap[scopeKeyAndIdentifierText] = typeDefinitionNode;
    }
    
    FunctionDefinitionNode[] IBinder.GetFunctionDefinitionNodesByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey) =>
	    GetFunctionDefinitionNodesByScope(model: null, resourceUri, scopeKey);
    
    public FunctionDefinitionNode[] GetFunctionDefinitionNodesByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    		return Array.Empty<FunctionDefinitionNode>();

    	return binderSession.ScopeFunctionDefinitionMap
    		.Where(kvp => kvp.Key.ScopeKey == scopeKey)
    		.Select(kvp => kvp.Value)
    		.ToArray();
    }
    
    bool IBinder.TryGetFunctionDefinitionNodeByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey,
	    	string functionIdentifierText,
	    	out FunctionDefinitionNode functionDefinitionNode) =>
    	TryGetFunctionDefinitionNodeByScope(model: null, resourceUri, scopeKey, functionIdentifierText, out functionDefinitionNode);
    
    public bool TryGetFunctionDefinitionNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
    	out FunctionDefinitionNode functionDefinitionNode)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    	{
    		functionDefinitionNode = null;
    		return false;
    	}
    		
    	var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, functionIdentifierText);
    	return binderSession.ScopeFunctionDefinitionMap.TryGetValue(scopeKeyAndIdentifierText, out functionDefinitionNode);
    }
    
    bool IBinder.TryAddFunctionDefinitionNodeByScope(
			ResourceUri resourceUri,
			Key<IScope> scopeKey,
			string functionIdentifierText,
		    FunctionDefinitionNode functionDefinitionNode) =>
		TryAddFunctionDefinitionNodeByScope(model: null, resourceUri, scopeKey, functionIdentifierText, functionDefinitionNode);
    
    public bool TryAddFunctionDefinitionNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    		return false;
    	
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, functionIdentifierText);
    	return binderSession.ScopeFunctionDefinitionMap.TryAdd(scopeKeyAndIdentifierText, functionDefinitionNode);
    }
    
    void IBinder.SetFunctionDefinitionNodeByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey,
	    	string functionIdentifierText,
	        FunctionDefinitionNode functionDefinitionNode) =>
	    SetFunctionDefinitionNodeByScope(model: null, resourceUri, scopeKey, functionIdentifierText, functionDefinitionNode);
    
    public void SetFunctionDefinitionNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    		return;
    	
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, functionIdentifierText);
    	binderSession.ScopeFunctionDefinitionMap[scopeKeyAndIdentifierText] = functionDefinitionNode;
    }

	IVariableDeclarationNode[] IBinder.GetVariableDeclarationNodesByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey) =>
	    GetVariableDeclarationNodesByScope(model: null, resourceUri, scopeKey);

    public IVariableDeclarationNode[] GetVariableDeclarationNodesByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    		return Array.Empty<IVariableDeclarationNode>();
    	
    	return binderSession.ScopeVariableDeclarationMap
    		.Where(kvp => kvp.Key.ScopeKey == scopeKey)
    		.Select(kvp => kvp.Value)
    		.ToArray();
    }
    
    bool IBinder.TryGetVariableDeclarationNodeByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey,
	    	string variableIdentifierText,
	    	out IVariableDeclarationNode variableDeclarationNode) =>
    	TryGetVariableDeclarationNodeByScope(model: null, resourceUri, scopeKey, variableIdentifierText, out variableDeclarationNode);
    
    public bool TryGetVariableDeclarationNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
    	out IVariableDeclarationNode variableDeclarationNode)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    	{
    		variableDeclarationNode = null;
    		return false;
    	}
    		
    	var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, variableIdentifierText);
    	return binderSession.ScopeVariableDeclarationMap.TryGetValue(scopeKeyAndIdentifierText, out variableDeclarationNode);
    }
    
    bool IBinder.TryAddVariableDeclarationNodeByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey,
	    	string variableIdentifierText,
	        IVariableDeclarationNode variableDeclarationNode) =>
	    TryAddVariableDeclarationNodeByScope(model: null, resourceUri, scopeKey, variableIdentifierText, variableDeclarationNode);
        
    public bool TryAddVariableDeclarationNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    		return false;
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, variableIdentifierText);
    	return binderSession.ScopeVariableDeclarationMap.TryAdd(scopeKeyAndIdentifierText, variableDeclarationNode);
    }
    
    void IBinder.SetVariableDeclarationNodeByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey,
	    	string variableIdentifierText,
	        IVariableDeclarationNode variableDeclarationNode) =>
	    SetVariableDeclarationNodeByScope(model: null, resourceUri, scopeKey, variableIdentifierText, variableDeclarationNode);
        
    public void SetVariableDeclarationNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    		return;
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, variableIdentifierText);
    	binderSession.ScopeVariableDeclarationMap[scopeKeyAndIdentifierText] = variableDeclarationNode;
    }
    
    TypeClauseNode? IBinder.GetReturnTypeClauseNodeByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey) =>
	    GetReturnTypeClauseNodeByScope(model: null, resourceUri, scopeKey);

    public TypeClauseNode? GetReturnTypeClauseNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey)
    {
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    		return null;
    	
    	if (binderSession.ScopeReturnTypeClauseNodeMap.TryGetValue(scopeKey, out var returnTypeClauseNode))
    		return returnTypeClauseNode;
    	else
    		return null;
    }
    
    bool IBinder.TryAddReturnTypeClauseNodeByScope(
	    	ResourceUri resourceUri,
	    	Key<IScope> scopeKey,
	        TypeClauseNode typeClauseNode) =>
	    TryAddReturnTypeClauseNodeByScope(model: null, resourceUri, scopeKey, typeClauseNode);
    
    public bool TryAddReturnTypeClauseNodeByScope(
    	IParserModel? model,
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
        TypeClauseNode typeClauseNode)
	{    	
    	if (!TryGetBinderSession(model, resourceUri, out var binderSession))
    	{
    		typeClauseNode = null;
    		return false;
    	}
    		
    	return binderSession.ScopeReturnTypeClauseNodeMap.TryAdd(scopeKey, typeClauseNode);
    }

    TextEditorTextSpan? IBinder.GetDefinition(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource) =>
    	GetDefinition(model: null, textSpan, compilerServiceResource);
    
    public TextEditorTextSpan? GetDefinition(IParserModel? model, TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource)
    {
        var boundScope = GetScope(model, textSpan);
        
        if (compilerServiceResource.CompilationUnit is null)
        	return null;
        
        // Try to find a symbol at that cursor position.
		var symbols = compilerServiceResource.GetSymbols();
		var foundSymbol = (ITextEditorSymbol?)null;
		
        foreach (var symbol in symbols)
        {
            if (textSpan.StartingIndexInclusive >= symbol.TextSpan.StartingIndexInclusive &&
                textSpan.StartingIndexInclusive < symbol.TextSpan.EndingIndexExclusive)
            {
                foundSymbol = symbol;
                break;
            }
        }
		
		if (foundSymbol is null)
			return null;
			
		var currentSyntaxKind = foundSymbol.SyntaxKind;
        
        switch (currentSyntaxKind)
        {
        	case SyntaxKind.VariableAssignmentExpressionNode:
        	case SyntaxKind.VariableDeclarationNode:
        	case SyntaxKind.VariableReferenceNode:
        	case SyntaxKind.VariableSymbol:
        	case SyntaxKind.PropertySymbol:
        	case SyntaxKind.FieldSymbol:
        	{
        		if (TryGetVariableDeclarationHierarchically(
        				model,
        				boundScope.ResourceUri,
        				boundScope.Key,
		                textSpan.GetText(),
		                out var variableDeclarationStatementNode)
		            && variableDeclarationStatementNode is not null)
		        {
		            return variableDeclarationStatementNode.IdentifierToken.TextSpan;
		        }
		        
		        return null;
        	}
        	case SyntaxKind.FunctionInvocationNode:
        	case SyntaxKind.FunctionDefinitionNode:
        	case SyntaxKind.FunctionSymbol:
	        {
	        	if (TryGetFunctionHierarchically(
	        				 model,
	        				 boundScope.ResourceUri,
        					 boundScope.Key,
		                     textSpan.GetText(),
		                     out var functionDefinitionNode)
		                 && functionDefinitionNode is not null)
		        {
		            return functionDefinitionNode.FunctionIdentifierToken.TextSpan;
		        }
		        
		        return null;
	        }
	        case SyntaxKind.TypeClauseNode:
	        case SyntaxKind.TypeDefinitionNode:
	        case SyntaxKind.TypeSymbol:
	        case SyntaxKind.ConstructorSymbol:
	        {
	        	if (TryGetTypeDefinitionHierarchically(
	        				 model,
	        			     boundScope.ResourceUri,
        					 boundScope.Key,
		                     textSpan.GetText(),
		                     out var typeDefinitionNode)
		                 && typeDefinitionNode is not null)
		        {
		            return typeDefinitionNode.TypeIdentifierToken.TextSpan;
		        }
		        
		        return null;
	        }
        }

        return null;
    }

    public ISyntaxNode? GetSyntaxNode(int positionIndex, CompilationUnit compilationUnit)
    {
        // First attempt at writing this, will be to start from the root of the compilation unit,
        // then traverse the syntax tree where the position index is within bounds.

        return RecursiveGetSyntaxNode(positionIndex, compilationUnit.RootCodeBlockNode);

        ISyntaxNode? RecursiveGetSyntaxNode(int positionIndex, ISyntaxNode targetNode)
        {
            foreach (var child in targetNode.ChildList)
            {
                if (child is ISyntaxNode syntaxNode)
                {
                    var innerResult = RecursiveGetSyntaxNode(positionIndex, syntaxNode);

                    if (innerResult is not null)
                        return innerResult;
                }
                else if (child is ISyntaxToken syntaxToken)
                {
                    if (syntaxToken.TextSpan.StartingIndexInclusive <= positionIndex &&
                        syntaxToken.TextSpan.EndingIndexExclusive >= positionIndex)
                    {
                        return targetNode;
                    }
                }
            }

            return null;
        }
    }
}
