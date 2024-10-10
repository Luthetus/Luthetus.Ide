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
    private readonly Dictionary<string, NamespaceGroupNode> _namespaceGroupNodeMap = CSharpFacts.Namespaces.GetInitialBoundNamespaceStatementNodes();
    /// <summary>
    /// The key for _symbolDefinitions is calculated by <see cref="ISymbol.GetSymbolDefinitionId"/>
    /// </summary>
    private readonly Dictionary<string, SymbolDefinition> _symbolDefinitions = new();
    /// <summary>
    /// All of the type definitions should be maintainted in this dictionary as they are
    /// found via parsing. Then, when one types an ambiguous identifier, perhaps they
    /// wanted a type, and a lookup in this map can be done, and a using statement
    /// inserted for the user if they decide to use that autocomplete option.
    /// </summary>
    private readonly Dictionary<NamespaceAndTypeIdentifiers, TypeDefinitionNode> _allTypeDefinitions = new();
    private readonly DiagnosticBag _diagnosticBag = new();
    private readonly IScope _globalScope = CSharpFacts.ScopeFacts.GetInitialGlobalScope();
    private readonly NamespaceStatementNode _topLevelNamespaceStatementNode = CSharpFacts.Namespaces.GetTopLevelNamespaceStatementNode();

    private readonly Dictionary<ResourceUri, List<IScope>> _boundScopes = new();
    /// <summary>
    /// Key is the name of the type, prefixed with the ScopeKey and '_' to separate the ScopeKey from the type.
    /// Given: public class MyClass { }
    /// Then: Key == new ScopeKeyAndIdentifierText(ScopeKey, "MyClass")
    /// </summary>
    private readonly Dictionary<ResourceUri, Dictionary<ScopeKeyAndIdentifierText, TypeDefinitionNode>> _scopeTypeDefinitionMap = new();
    /// <summary>
    /// Key is the name of the function, prefixed with the ScopeKey and '_' to separate the ScopeKey from the function.
    /// Given: public void MyMethod() { }
    /// Then: Key == new ScopeKeyAndIdentifierText(ScopeKey, "MyMethod")
    /// </summary>
    private readonly Dictionary<ResourceUri, Dictionary<ScopeKeyAndIdentifierText, FunctionDefinitionNode>> _scopeFunctionDefinitionMap = new();
    /// <summary>
    /// Key is the name of the variable, prefixed with the ScopeKey and '_' to separate the ScopeKey from the variable.
    /// Given: var myVariable = 2;
    /// Then: Key == new ScopeKeyAndIdentifierText(ScopeKey, "myVariable")
    /// </summary>
    private readonly Dictionary<ResourceUri, Dictionary<ScopeKeyAndIdentifierText, IVariableDeclarationNode>> _scopeVariableDeclarationMap = new();
    private readonly Dictionary<ResourceUri, Dictionary<Key<IScope>, TypeClauseNode>> _scopeReturnTypeClauseNodeMap = new();
    
    public CSharpBinder()
    {
        _boundScopes.Add(_globalScope.ResourceUri, new List<IScope> { _globalScope });
    }

    public ImmutableDictionary<string, NamespaceGroupNode> NamespaceGroupNodes => _namespaceGroupNodeMap.ToImmutableDictionary();
    public ImmutableArray<ISymbol> Symbols => _symbolDefinitions.Values.SelectMany(x => x.SymbolReferences).Select(x => x.Symbol).ToImmutableArray();
    public Dictionary<string, SymbolDefinition> SymbolDefinitions => _symbolDefinitions;
    public ImmutableDictionary<ResourceUri, List<IScope>> ScopeList => _boundScopes.ToImmutableDictionary();
    public ImmutableDictionary<NamespaceAndTypeIdentifiers, TypeDefinitionNode> AllTypeDefinitions => _allTypeDefinitions.ToImmutableDictionary();
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList => _diagnosticBag.ToImmutableArray();

    ImmutableArray<ITextEditorSymbol> IBinder.SymbolsList => Symbols
        .Select(s => (ITextEditorSymbol)s)
        .ToImmutableArray();

    public IBinderSession ConstructBinderSession(ResourceUri resourceUri)
    {
    	RegisterStateByResourceUri(resourceUri);
    
        return new CSharpBinderSession(
            resourceUri,
            _globalScope.Key,
            _topLevelNamespaceStatementNode,
            this);
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
        		model.BinderSession.ResourceUri,
        		model.BinderSession.CurrentScopeKey,
        		functionIdentifierText,
                functionDefinitionNode))
        {
            _diagnosticBag.ReportAlreadyDefinedFunction(
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

            _diagnosticBag.ReportBadFunctionOptionalArgumentDueToMismatchInType(
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
                	model.BinderSession.ResourceUri,
        			model.BinderSession.CurrentScopeKey,
                	text,
                	variableDeclarationNode);
            }

            _diagnosticBag.ReportAlreadyDefinedVariable(
                variableDeclarationNode.IdentifierToken.TextSpan,
                text);
        }
        else
        {
        	_ = TryAddVariableDeclarationNodeByScope(
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

            _diagnosticBag.ReportUndefinedVariable(
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
                _diagnosticBag.TheNameDoesNotExistInTheCurrentContext(
                    variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan,
                    text);
            }
            else
            {
                _diagnosticBag.ReportUndefinedVariable(
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
            _diagnosticBag.ReportUndefinedFunction(
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
        	Key: Key<IScope>.NewKey(),
		    ParentKey: model.BinderSession.CurrentScopeKey,
		    model.BinderSession.ResourceUri,
		    textSpan.StartingIndexInclusive,
		    EndingIndexExclusive: null);
            
        if (!_boundScopes.ContainsKey(scope.ResourceUri))
        	throw new NotImplementedException(nameof(RegisterScope));
        	
        _boundScopes[scope.ResourceUri].Add(scope);

        _boundScopes[scope.ResourceUri] = _boundScopes[scope.ResourceUri]
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();

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
    
    	if (!_boundScopes.TryGetValue(model.BinderSession.ResourceUri, out var narrowToFile))
    		return;
    	
    	var indexOf = narrowToFile.FindIndex(x => x.Key == model.BinderSession.CurrentScopeKey);

        if (indexOf == -1)
        {
        	Console.WriteLine(narrowToFile.Count);
            throw new NotImplementedException();
        }
        
    	var scope = (Scope)narrowToFile[indexOf];
    	narrowToFile[indexOf] = scope with
    	{
    		EndingIndexExclusive = textSpan.EndingIndexExclusive
    	};

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
        		model.BinderSession.ResourceUri,
        		model.BinderSession.CurrentScopeKey,
        		typeIdentifierText,
        		out var existingTypeDefinitionNode))
        {
        	if (shouldOverwrite || existingTypeDefinitionNode.IsFabricated)
        	{
        		SetTypeDefinitionNodeByScope(
        			model.BinderSession.ResourceUri,
	        		model.BinderSession.CurrentScopeKey,
	        		typeIdentifierText,
	        		typeDefinitionNode);
        	}
        }
        else
        {
        	_ = TryAddTypeDefinitionNodeByScope(
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

        _boundScopes.Remove(resourceUri);

		_scopeTypeDefinitionMap.Remove(resourceUri);
		_scopeTypeDefinitionMap.Remove(resourceUri);
		_scopeVariableDeclarationMap.Remove(resourceUri);
		_scopeReturnTypeClauseNodeMap.Remove(resourceUri);

        _diagnosticBag.ClearByResourceUri(resourceUri);
    }
    
    public void RegisterStateByResourceUri(ResourceUri resourceUri)
    {
        _ = _boundScopes.TryAdd(resourceUri, new());

		_ = _scopeTypeDefinitionMap.TryAdd(resourceUri, new());
		_ = _scopeTypeDefinitionMap.TryAdd(resourceUri, new());
		_ = _scopeVariableDeclarationMap.TryAdd(resourceUri, new());
		_ = _scopeReturnTypeClauseNodeMap.TryAdd(resourceUri, new());
    }

    /// <summary>
    /// Search hierarchically through all the scopes, starting at the <see cref="initialScope"/>.<br/><br/>
    /// If a match is found, then set the out parameter to it and return true.<br/><br/>
    /// If none of the searched scopes contained a match then set the out parameter to null and return false.
    /// </summary>
    public bool TryGetFunctionHierarchically(
        ResourceUri resourceUri,
    	Key<IScope> initialScopeKey,
        string identifierText,
        out FunctionDefinitionNode? functionDefinitionNode)
    {
        var localScope = GetScope(resourceUri, initialScopeKey);

        while (localScope is not null)
        {
            if (TryGetFunctionDefinitionNodeByScope(
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
            	localScope = GetScope(resourceUri, localScope.ParentKey.Value);
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
        ResourceUri resourceUri,
    	Key<IScope> initialScopeKey,
        string identifierText,
        out TypeDefinitionNode? typeDefinitionNode)
    {
        var localScope = GetScope(resourceUri, initialScopeKey);

        while (localScope is not null)
        {
            if (TryGetTypeDefinitionNodeByScope(
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
            	localScope = GetScope(resourceUri, localScope.ParentKey.Value);
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
    	ResourceUri resourceUri,
    	Key<IScope> initialScopeKey,
        string identifierText,
        out IVariableDeclarationNode? variableDeclarationStatementNode)
    {
        var localScope = GetScope(resourceUri, initialScopeKey);

        while (localScope is not null)
        {
            if (TryGetVariableDeclarationNodeByScope(
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
            	localScope = GetScope(resourceUri, localScope.ParentKey.Value);
        }

        variableDeclarationStatementNode = null;
        return false;
    }

    public IScope? GetScope(TextEditorTextSpan textSpan)
    {
    	var totalFile = new List<IScope>();
    
    	if (_boundScopes.TryGetValue(textSpan.ResourceUri, out var selfFile))
    		totalFile.AddRange(selfFile);
		if (_boundScopes.TryGetValue(ResourceUri.Empty, out var globalFile))
    		totalFile.AddRange(globalFile);
        
        var possibleScopes = totalFile.Where(x =>
        {
            return x.StartingIndexInclusive <= textSpan.StartingIndexInclusive &&
            	   // Global Scope awkwardly has a null ending index exclusive (2023-10-15)
                   (x.EndingIndexExclusive >= textSpan.StartingIndexInclusive || x.EndingIndexExclusive is null);
        });

        return possibleScopes.MinBy(x => textSpan.StartingIndexInclusive - x.StartingIndexInclusive);
    }
    
    public IScope? GetScope(ResourceUri resourceUri, int positionIndex)
    {
    	var totalFile = new List<IScope>();
    
    	if (_boundScopes.TryGetValue(resourceUri, out var selfFile))
    		totalFile.AddRange(selfFile);
		if (_boundScopes.TryGetValue(ResourceUri.Empty, out var globalFile))
    		totalFile.AddRange(globalFile);
        
        var possibleScopes = totalFile.Where(x =>
            x.StartingIndexInclusive <= positionIndex);

        return possibleScopes.MinBy(x => positionIndex - x.StartingIndexInclusive);
    }
    
    public IScope? GetScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
    	var totalFile = new List<IScope>();
    
    	if (_boundScopes.TryGetValue(resourceUri, out var selfFile))
    		totalFile.AddRange(selfFile);
		if (_boundScopes.TryGetValue(ResourceUri.Empty, out var globalFile))
    		totalFile.AddRange(globalFile);
        
        return totalFile.FirstOrDefault(x => x.Key == scopeKey);
    }
    
    public IScope[]? GetScopeList(ResourceUri resourceUri)
    {
    	var totalFile = new List<IScope>();
    
    	if (_boundScopes.TryGetValue(resourceUri, out var selfFile))
    		totalFile.AddRange(selfFile);
		if (_boundScopes.TryGetValue(ResourceUri.Empty, out var globalFile))
    		totalFile.AddRange(globalFile);
    		
    	return totalFile.ToArray();
    }
    
    public TypeDefinitionNode[] GetTypeDefinitionNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
    	if (!_scopeTypeDefinitionMap.TryGetValue(resourceUri, out var narrowDownToFile))
    		return Array.Empty<TypeDefinitionNode>();
    	
    	var narrowDownToScope = narrowDownToFile.Where(kvp => kvp.Key.ScopeKey == scopeKey);
    	return narrowDownToScope.Select(kvp => kvp.Value).ToArray();
    }
    
    public bool TryGetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
    	out TypeDefinitionNode typeDefinitionNode)
    {
    	if (!_scopeTypeDefinitionMap.TryGetValue(resourceUri, out var narrowDownToFile))
    	{
    		typeDefinitionNode = null;
    		return false;
    	}
    	
    	var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, typeIdentifierText);
    	return narrowDownToFile.TryGetValue(scopeKeyAndIdentifierText, out typeDefinitionNode);
    }
    
    public bool TryAddTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	if (!_scopeTypeDefinitionMap.TryGetValue(resourceUri, out var narrowDownToFile))
    	{
    		return false;
    	}
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, typeIdentifierText);
    	return narrowDownToFile.TryAdd(scopeKeyAndIdentifierText, typeDefinitionNode);
    }
    
    public void SetTypeDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	if (!_scopeTypeDefinitionMap.TryGetValue(resourceUri, out var narrowDownToFile))
    		return;
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, typeIdentifierText);
    	narrowDownToFile[scopeKeyAndIdentifierText] = typeDefinitionNode;
    }
    
    public FunctionDefinitionNode[] GetFunctionDefinitionNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
    	if (!_scopeFunctionDefinitionMap.TryGetValue(resourceUri, out var narrowDownToFile))
    		return Array.Empty<FunctionDefinitionNode>();

    	var narrowDownToScope = narrowDownToFile.Where(kvp => kvp.Key.ScopeKey == scopeKey);
    	return narrowDownToScope.Select(kvp => kvp.Value).ToArray();
    }
    
    public bool TryGetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
    	out FunctionDefinitionNode functionDefinitionNode)
    {
    	if (!_scopeFunctionDefinitionMap.TryGetValue(resourceUri, out var narrowDownToFile))
    	{
    		functionDefinitionNode = null;
    		return false;
    	}
    		
    	var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, functionIdentifierText);
    	return narrowDownToFile.TryGetValue(scopeKeyAndIdentifierText, out functionDefinitionNode);
    }
    
    public bool TryAddFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	if (!_scopeFunctionDefinitionMap.TryGetValue(resourceUri, out var narrowDownToFile))
		{
			return false;
		}
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, functionIdentifierText);
    	return narrowDownToFile.TryAdd(scopeKeyAndIdentifierText, functionDefinitionNode);
    }
    
    public void SetFunctionDefinitionNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	if (!_scopeFunctionDefinitionMap.TryGetValue(resourceUri, out var narrowDownToFile))
    		return;
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, functionIdentifierText);
    	narrowDownToFile[scopeKeyAndIdentifierText] = functionDefinitionNode;
    }

    public IVariableDeclarationNode[] GetVariableDeclarationNodesByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
    	if (!_scopeVariableDeclarationMap.TryGetValue(resourceUri, out var narrowDownToFile))
    		return Array.Empty<IVariableDeclarationNode>();
    		
    	var narrowDownToScope = narrowDownToFile.Where(kvp => kvp.Key.ScopeKey == scopeKey);
    	return narrowDownToScope.Select(kvp => kvp.Value).ToArray();
    }
    
    public bool TryGetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
    	out IVariableDeclarationNode variableDeclarationNode)
    {
    	if (!_scopeVariableDeclarationMap.TryGetValue(resourceUri, out var narrowDownToFile))
    	{
    		variableDeclarationNode = null;
    		return false;
    	}
    		
    	var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, variableIdentifierText);
    	return narrowDownToFile.TryGetValue(scopeKeyAndIdentifierText, out variableDeclarationNode);
    }
    
    public bool TryAddVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode)
    {
    	if (!_scopeVariableDeclarationMap.TryGetValue(resourceUri, out var narrowDownToFile))
    		return false;
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, variableIdentifierText);
    	return narrowDownToFile.TryAdd(scopeKeyAndIdentifierText, variableDeclarationNode);
    }
    
    public void SetVariableDeclarationNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode)
    {
    	if (!_scopeVariableDeclarationMap.TryGetValue(resourceUri, out var narrowDownToFile))
    		return;
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeKey, variableIdentifierText);
    	narrowDownToFile[scopeKeyAndIdentifierText] = variableDeclarationNode;
    }

    public TypeClauseNode? GetReturnTypeClauseNodeByScope(ResourceUri resourceUri, Key<IScope> scopeKey)
    {
    	if (!_scopeReturnTypeClauseNodeMap.TryGetValue(resourceUri, out var narrowDownToFile))
    		return null;
    	
    	if (narrowDownToFile.TryGetValue(scopeKey, out var returnTypeClauseNode))
    		return returnTypeClauseNode;
    	else
    		return null;
    }
    
    public bool TryAddReturnTypeClauseNodeByScope(
    	ResourceUri resourceUri,
    	Key<IScope> scopeKey,
        TypeClauseNode typeClauseNode)
    {
    	if (!_scopeReturnTypeClauseNodeMap.TryGetValue(resourceUri, out var narrowDownToFile))
    	{
    		typeClauseNode = null;
    		return false;
    	}
    		
    	return narrowDownToFile.TryAdd(scopeKey, typeClauseNode);
    }

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource)
    {
        var boundScope = GetScope(textSpan);
        
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
