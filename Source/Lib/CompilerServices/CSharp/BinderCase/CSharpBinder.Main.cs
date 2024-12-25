using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
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
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.CompilerServices.CSharp.BinderCase;

public partial class CSharpBinder : IBinder
{
	private readonly Dictionary<ResourceUri, CSharpBinderSession> _binderSessionMap = new();
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
    	#if DEBUG
    	++LuthetusDebugSomething.Binder_ConstructorInvocationCount;
    	#endif

    	var globalBinderSession = StartBinderSession(ResourceUri.Empty);
    	globalBinderSession.ScopeList.Add(_globalScope);
    	FinalizeBinderSession(globalBinderSession);
        // _boundScopes.Add(_globalScope.ResourceUri, new List<IScope> { _globalScope });
    }

    public IReadOnlyDictionary<string, NamespaceGroupNode> NamespaceGroupNodes => _namespaceGroupNodeMap;
    public Dictionary<string, SymbolDefinition> SymbolDefinitions => _symbolDefinitions;
    public IReadOnlyDictionary<NamespaceAndTypeIdentifiers, TypeDefinitionNode> AllTypeDefinitions => _allTypeDefinitions;
    public TextEditorDiagnostic[] DiagnosticsList => Array.Empty<TextEditorDiagnostic>();
    
    ITextEditorSymbol[] IBinder.SymbolsList => Symbols;
    
    /// <summary>
    /// This will return an empty array if the collection is modified during enumeration
    /// (specifically this is catching 'InvalidOperationException').
    /// </summary>
    public ITextEditorSymbol[] Symbols
    {
    	get
    	{
    		try
    		{
    			return _symbolDefinitions.Values.SelectMany(x => x.SymbolReferences).Select(x => x.Symbol).ToArray();
    		}
    		catch (InvalidOperationException e)
    		{
    			return Array.Empty<ITextEditorSymbol>();
    		}
    	}
    }
    
    IBinderSession IBinder.StartBinderSession(ResourceUri resourceUri) =>
    	StartBinderSession(resourceUri);

	/// <summary><see cref="FinalizeBinderSession"/></summary>
    public CSharpBinderSession StartBinderSession(ResourceUri resourceUri)
    {
    	foreach (var namespaceGroupNodeKvp in _namespaceGroupNodeMap)
        {
        	for (int i = namespaceGroupNodeKvp.Value.NamespaceStatementNodeList.Count - 1; i >= 0; i--)
        	{
        		var x = namespaceGroupNodeKvp.Value.NamespaceStatementNodeList[i];
        		
        		if (x.IdentifierToken.TextSpan.ResourceUri == resourceUri)
        			namespaceGroupNodeKvp.Value.NamespaceStatementNodeList.RemoveAt(i);
        	}
        }

        foreach (var symbolDefinition in _symbolDefinitions)
        {
            for (int i = symbolDefinition.Value.SymbolReferences.Count - 1; i >= 0; i--)
        	{
        		var x = symbolDefinition.Value.SymbolReferences[i];
        		
        		if (x.Symbol.TextSpan.ResourceUri == resourceUri)
        			symbolDefinition.Value.SymbolReferences.RemoveAt(i);
        	}
        }
    	
        var cSharpBinderSession = new CSharpBinderSession(
            resourceUri,
            this,
            0,
            _topLevelNamespaceStatementNode);
            
        cSharpBinderSession.ScopeList.Add(_globalScope);
        
        return cSharpBinderSession;
    }

	void IBinder.FinalizeBinderSession(IBinderSession binderSession) =>
		FinalizeBinderSession((CSharpBinderSession)binderSession);

	/// <summary><see cref="StartBinderSession"/></summary>
	public void FinalizeBinderSession(CSharpBinderSession binderSession)
	{
		UpsertBinderSession(binderSession);
	}

    public LiteralExpressionNode BindLiteralExpressionNode(
        LiteralExpressionNode literalExpressionNode,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
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
            	parserModel.DiagnosticBag.ReportTodoException(literalExpressionNode.LiteralSyntaxToken.TextSpan, $"{nameof(BindLiteralExpressionNode)}(...) failed to map SyntaxKind: '{literalExpressionNode.LiteralSyntaxToken.SyntaxKind}'");
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
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
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
        CSharpCompilationUnit compilationUnit)
    {
        AddSymbolReference(
        	new StringInterpolationSymbol(
	        	compilationUnit.BinderSession.GetNextSymbolId(),
	        	dollarSignToken.TextSpan with
		        {
		            DecorationByte = (byte)GenericDecorationKind.StringLiteral,
		        }),
			compilationUnit);
    }
    
    public void BindStringVerbatimExpression(
        AtToken atToken,
        CSharpCompilationUnit compilationUnit)
    {
        AddSymbolReference(
        	new StringVerbatimSymbol(
        		compilationUnit.BinderSession.GetNextSymbolId(),
        		atToken.TextSpan with
		        {
		            DecorationByte = (byte)GenericDecorationKind.StringLiteral,
		        }),
		    compilationUnit);
    }
    
    public void BindDiscard(
        IdentifierToken identifierToken,
        CSharpCompilationUnit compilationUnit)
    {
        AddSymbolReference(
        	new DiscardSymbol(
	        	compilationUnit.BinderSession.GetNextSymbolId(),
	        	identifierToken.TextSpan with
		        {
		            DecorationByte = (byte)GenericDecorationKind.None,
		        }),
			compilationUnit);
    }

    public void BindFunctionDefinitionNode(
        FunctionDefinitionNode functionDefinitionNode,
        CSharpCompilationUnit compilationUnit)
    {
        var functionIdentifierText = functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText();

        var functionSymbol = new FunctionSymbol(
        	compilationUnit.BinderSession.GetNextSymbolId(),
        	functionDefinitionNode.FunctionIdentifierToken.TextSpan with
	        {
	            DecorationByte = (byte)GenericDecorationKind.Function
	        });

        AddSymbolDefinition(functionSymbol, compilationUnit);

        if (!TryAddFunctionDefinitionNodeByScope(
        		compilationUnit,
        		compilationUnit.BinderSession.ResourceUri,
        		compilationUnit.BinderSession.CurrentScopeIndexKey,
        		functionIdentifierText,
                functionDefinitionNode))
        {
            compilationUnit.BinderSession.DiagnosticBag.ReportAlreadyDefinedFunction(
                functionDefinitionNode.FunctionIdentifierToken.TextSpan,
                functionIdentifierText);
        }
    }
    
    void IBinder.BindFunctionOptionalArgument(FunctionArgumentEntryNode functionArgumentEntryNode, IParserModel parserModel) =>
    	BindFunctionOptionalArgument(functionArgumentEntryNode, compilationUnit: null);
    
    public void BindFunctionOptionalArgument(
        FunctionArgumentEntryNode functionArgumentEntryNode,
        CSharpCompilationUnit? compilationUnit)
    {
    	if (compilationUnit is null)
    		return;
    
        var argumentTypeClauseNode = functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode;
        
        /*
		// TODO: Wouldn't this have a '!' at the start? And '... && typeDefinitionNode is not null' (2024-10-25)
        if (TryGetTypeDefinitionHierarchically(
        		model,
        		compilationUnit.BinderSession.ResourceUri,
                compilationUnit.BinderSession.CurrentScopeIndexKey,
                argumentTypeClauseNode.TypeIdentifierToken.TextSpan.GetText(),
                out var typeDefinitionNode)
            || typeDefinitionNode is null)
        {
            typeDefinitionNode = CSharpFacts.Types.Void;
        }

        var literalExpressionNode = new LiteralExpressionNode(
            functionArgumentEntryNode.OptionalCompileTimeConstantToken,
            typeDefinitionNode.ToTypeClause());

        literalExpressionNode = BindLiteralExpressionNode(literalExpressionNode, compilationUnit);

        if (literalExpressionNode.ResultTypeClauseNode.ValueType is null ||
            literalExpressionNode.ResultTypeClauseNode.ValueType != functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode.ValueType)
        {
            var optionalArgumentTextSpan = functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode.TypeIdentifierToken.TextSpan with
            {
                EndingIndexExclusive = functionArgumentEntryNode.VariableDeclarationNode.IdentifierToken.TextSpan.EndingIndexExclusive
            };

            compilationUnit.BinderSession.DiagnosticBag.ReportBadFunctionOptionalArgumentDueToMismatchInType(
                optionalArgumentTextSpan,
                functionArgumentEntryNode.VariableDeclarationNode.IdentifierToken.TextSpan.GetText(),
                functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode.ValueType?.Name ?? "null",
                literalExpressionNode.ResultTypeClauseNode.ValueType?.Name ?? "null");
        }
        */
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
        CSharpCompilationUnit compilationUnit)
    {
        compilationUnit.BinderSession.CurrentNamespaceStatementNode = namespaceStatementNode;
    }

    public void BindNamespaceStatementNode(
        NamespaceStatementNode namespaceStatementNode,
        CSharpCompilationUnit compilationUnit)
    {
        var namespaceString = namespaceStatementNode.IdentifierToken.TextSpan.GetText();
        
        AddSymbolReference(
        	new NamespaceSymbol(
        		compilationUnit.BinderSession.GetNextSymbolId(),
        		namespaceStatementNode.IdentifierToken.TextSpan),
        	compilationUnit);

        if (_namespaceGroupNodeMap.TryGetValue(namespaceString, out var inNamespaceGroupNode))
        {
        	var outNamespaceStatementNodeList = new List<NamespaceStatementNode>(inNamespaceGroupNode.NamespaceStatementNodeList);
            outNamespaceStatementNodeList.Add(namespaceStatementNode);

            var outNamespaceGroupNode = new NamespaceGroupNode(
                inNamespaceGroupNode.NamespaceString,
                outNamespaceStatementNodeList);

            _namespaceGroupNodeMap[namespaceString] = outNamespaceGroupNode;
        }
        else
        {
            _namespaceGroupNodeMap.Add(namespaceString, new NamespaceGroupNode(
                namespaceString,
                new List<NamespaceStatementNode> { namespaceStatementNode }));
        }
    }

    public void BindConstructorInvocationNode()
    {
        // Deleted what was in this method because it was nonsense, and causing errors. (2023-08-06)
    }

    public InheritanceStatementNode BindInheritanceStatementNode(
        TypeClauseNode typeClauseNode,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
        AddSymbolReference(
        	new TypeSymbol(
	        	compilationUnit.BinderSession.GetNextSymbolId(),
	        	typeClauseNode.TypeIdentifierToken.TextSpan with
		        {
		            DecorationByte = (byte)GenericDecorationKind.Type
		        }),
	        compilationUnit);

        parserModel.DiagnosticBag.ReportTodoException(
            typeClauseNode.TypeIdentifierToken.TextSpan,
            $"Implement {nameof(BindInheritanceStatementNode)}");

        return new InheritanceStatementNode(typeClauseNode);
    }

	void IBinder.BindVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode, IParserModel parserModel) =>
		BindVariableDeclarationNode(variableDeclarationNode, compilationUnit: null);

    public void BindVariableDeclarationNode(
        IVariableDeclarationNode variableDeclarationNode,
        CSharpCompilationUnit compilationUnit)
    {
        CreateVariableSymbol(variableDeclarationNode.IdentifierToken, variableDeclarationNode.VariableKind, compilationUnit);
        var text = variableDeclarationNode.IdentifierToken.TextSpan.GetText();
        
        if (TryGetVariableDeclarationNodeByScope(
        		compilationUnit,
        		compilationUnit.BinderSession.ResourceUri,
        		compilationUnit.BinderSession.CurrentScopeIndexKey,
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
        			compilationUnit,
                	compilationUnit.BinderSession.ResourceUri,
        			compilationUnit.BinderSession.CurrentScopeIndexKey,
                	text,
                	variableDeclarationNode);
            }

            compilationUnit.BinderSession.DiagnosticBag.ReportAlreadyDefinedVariable(
                variableDeclarationNode.IdentifierToken.TextSpan,
                text);
        }
        else
        {
        	_ = TryAddVariableDeclarationNodeByScope(
        		compilationUnit,
        		compilationUnit.BinderSession.ResourceUri,
    			compilationUnit.BinderSession.CurrentScopeIndexKey,
            	text,
            	variableDeclarationNode);
        }
    }
    
    public bool RemoveVariableDeclarationNodeFromActiveBinderSession(
    	int scopeIndexKey, VariableDeclarationNode variableDeclarationNode, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	var text = variableDeclarationNode.IdentifierToken.TextSpan.GetText();
    	
    	if (TryGetVariableDeclarationNodeByScope(
        		compilationUnit,
        		compilationUnit.BinderSession.ResourceUri,
        		scopeIndexKey,
        		text,
        		out var existingVariableDeclarationNode))
        {
            var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeIndexKey, text);
	    	return compilationUnit.BinderSession.ScopeVariableDeclarationMap.Remove(scopeKeyAndIdentifierText);
        }
        
        return false;
    }

    public VariableReferenceNode ConstructAndBindVariableReferenceNode(
        IdentifierToken variableIdentifierToken,
        CSharpCompilationUnit compilationUnit)
    {
        var text = variableIdentifierToken.TextSpan.GetText();
        VariableReferenceNode? variableReferenceNode;

        if (TryGetVariableDeclarationHierarchically(
        		compilationUnit,
                compilationUnit.BinderSession.ResourceUri,
                compilationUnit.BinderSession.CurrentScopeIndexKey,
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

            compilationUnit.BinderSession.DiagnosticBag.ReportUndefinedVariable(
                variableIdentifierToken.TextSpan,
                text);
        }

        CreateVariableSymbol(variableReferenceNode.VariableIdentifierToken, variableDeclarationNode.VariableKind, compilationUnit);
        return variableReferenceNode;
    }

    public void BindVariableAssignmentExpressionNode(
        VariableAssignmentExpressionNode variableAssignmentExpressionNode,
        CSharpCompilationUnit compilationUnit)
    {
        var text = variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan.GetText();
        VariableKind variableKind = VariableKind.Local;

        if (TryGetVariableDeclarationHierarchically(
        		compilationUnit,
                compilationUnit.BinderSession.ResourceUri,
                compilationUnit.BinderSession.CurrentScopeIndexKey,
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
                compilationUnit.BinderSession.DiagnosticBag.TheNameDoesNotExistInTheCurrentContext(
                    variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan,
                    text);
            }
            else
            {
                compilationUnit.BinderSession.DiagnosticBag.ReportUndefinedVariable(
                    variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan,
                    text);
            }
        }

        CreateVariableSymbol(variableAssignmentExpressionNode.VariableIdentifierToken, variableKind, compilationUnit);
    }

    public void BindConstructorDefinitionIdentifierToken(
        IdentifierToken identifierToken,
        CSharpCompilationUnit compilationUnit)
    {
        var constructorSymbol = new ConstructorSymbol(
	        compilationUnit.BinderSession.GetNextSymbolId(),
	        identifierToken.TextSpan with
	        {
	            DecorationByte = (byte)GenericDecorationKind.Type
	        });

        AddSymbolDefinition(constructorSymbol, compilationUnit);
    }

    public void BindFunctionInvocationNode(
        FunctionInvocationNode functionInvocationNode,
        CSharpCompilationUnit compilationUnit)
    {
        var functionInvocationIdentifierText = functionInvocationNode
            .FunctionInvocationIdentifierToken.TextSpan.GetText();

        var functionSymbol = new FunctionSymbol(
        	compilationUnit.BinderSession.GetNextSymbolId(),
        	functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan with
	        {
	            DecorationByte = (byte)GenericDecorationKind.Function
	        });

        AddSymbolReference(functionSymbol, compilationUnit);

        if (TryGetFunctionHierarchically(
        		compilationUnit,
                compilationUnit.BinderSession.ResourceUri,
                compilationUnit.BinderSession.CurrentScopeIndexKey,
                functionInvocationIdentifierText,
                out var functionDefinitionNode) &&
            functionDefinitionNode is not null)
        {
            return;
        }
        else
        {
            compilationUnit.BinderSession.DiagnosticBag.ReportUndefinedFunction(
                functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan,
                functionInvocationIdentifierText);
        }
    }

    public void BindNamespaceReference(
        IdentifierToken namespaceIdentifierToken,
        CSharpCompilationUnit compilationUnit)
    {
        var namespaceSymbol = new NamespaceSymbol(
        	compilationUnit.BinderSession.GetNextSymbolId(),
        	namespaceIdentifierToken.TextSpan with
	        {
	            DecorationByte = (byte)GenericDecorationKind.None
	        });

        AddSymbolReference(namespaceSymbol, compilationUnit);
    }

    public void BindTypeClauseNode(
        TypeClauseNode typeClauseNode,
        CSharpCompilationUnit compilationUnit)
    {
        if (!typeClauseNode.IsKeywordType)
        {
            var typeSymbol = new TypeSymbol(
            	compilationUnit.BinderSession.GetNextSymbolId(),
            	typeClauseNode.TypeIdentifierToken.TextSpan with
	            {
	                DecorationByte = (byte)GenericDecorationKind.Type
	            });

            AddSymbolReference(typeSymbol, compilationUnit);
        }

        var matchingTypeDefintionNode = CSharpFacts.Types.TypeDefinitionNodes.SingleOrDefault(
            x => x.TypeIdentifierToken.TextSpan.GetText() == typeClauseNode.TypeIdentifierToken.TextSpan.GetText());

        if (matchingTypeDefintionNode is not null)
        {
        	typeClauseNode.SetValueType(matchingTypeDefintionNode.ValueType);
        }
    }

    public void BindTypeIdentifier(
        IdentifierToken identifierToken,
        CSharpCompilationUnit compilationUnit)
    {
        if (identifierToken.SyntaxKind == SyntaxKind.IdentifierToken)
        {
            var typeSymbol = new TypeSymbol(
            	compilationUnit.BinderSession.GetNextSymbolId(),
            	identifierToken.TextSpan with
	            {
	                DecorationByte = (byte)GenericDecorationKind.Type
	            });

            AddSymbolReference(typeSymbol, compilationUnit);
        }
    }

    public void BindUsingStatementNode(
        UsingStatementNode usingStatementNode,
        CSharpCompilationUnit compilationUnit)
    {
        AddSymbolReference(
        	new NamespaceSymbol(
        		compilationUnit.BinderSession.GetNextSymbolId(),
        		usingStatementNode.NamespaceIdentifier.TextSpan),
        	compilationUnit);

        compilationUnit.BinderSession.CurrentUsingStatementNodeList.Add(usingStatementNode);
        AddNamespaceToCurrentScope(usingStatementNode.NamespaceIdentifier.TextSpan.GetText(), compilationUnit);
    }

    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
    public AttributeNode BindAttributeNode(
        OpenSquareBracketToken openSquareBracketToken,
        List<ISyntaxToken> innerTokens,
        CloseSquareBracketToken closeSquareBracketToken,
        CSharpCompilationUnit compilationUnit)
    {
        AddSymbolReference(
        	new TypeSymbol(
        		compilationUnit.BinderSession.GetNextSymbolId(),
        		openSquareBracketToken.TextSpan with
		        {
		            DecorationByte = (byte)GenericDecorationKind.Type,
		            EndingIndexExclusive = closeSquareBracketToken.TextSpan.EndingIndexExclusive
		        }),
			compilationUnit);

        return new AttributeNode(
            openSquareBracketToken,
            innerTokens,
            closeSquareBracketToken);
    }

    public void OpenScope(
    	ICodeBlockOwner codeBlockOwner,
        TypeClauseNode? scopeReturnTypeClauseNode,
        TextEditorTextSpan textSpan,
        CSharpCompilationUnit compilationUnit)
    {
        var scope = new Scope(
        	codeBlockOwner,
        	indexKey: compilationUnit.BinderSession.GetNextIndexKey(),
		    parentIndexKey: compilationUnit.BinderSession.CurrentScopeIndexKey,
		    textSpan.StartingIndexInclusive,
		    endingIndexExclusive: null);

        compilationUnit.BinderSession.ScopeList.Insert(scope.IndexKey, scope);
        compilationUnit.BinderSession.CurrentScopeIndexKey = scope.IndexKey;
    }

	public void AddNamespaceToCurrentScope(string namespaceString, IParserModel parserModel) =>
		AddNamespaceToCurrentScope(namespaceString, compilationUnit: null);

    public void AddNamespaceToCurrentScope(
        string namespaceString,
        CSharpCompilationUnit? compilationUnit)
    {
    	if (compilationUnit is null)
    		return;
    	
        if (_namespaceGroupNodeMap.TryGetValue(namespaceString, out var namespaceGroupNode) &&
            namespaceGroupNode is not null)
        {
            var typeDefinitionNodes = namespaceGroupNode.GetTopLevelTypeDefinitionNodes();

            foreach (var typeDefinitionNode in typeDefinitionNodes)
            {
            	_ = TryAddTypeDefinitionNodeByScope(
        				compilationUnit,
	            		compilationUnit.BinderSession.ResourceUri,
	            		compilationUnit.BinderSession.CurrentScopeIndexKey,
	            		typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText(),
	            		typeDefinitionNode);
            }
        }
    }

    public void CloseScope(
        TextEditorTextSpan textSpan,
        CSharpCompilationUnit compilationUnit,
        ref CSharpParserModel parserModel)
    {
    	// Check if it is the global scope, if so return early.
    	{
	    	if (compilationUnit.BinderSession.CurrentScopeIndexKey == 0)
	    		return;
    	}
    	
    	var inBuilder = parserModel.CurrentCodeBlockBuilder;
    	var inOwner = inBuilder.CodeBlockOwner;
    	
    	var outBuilder = parserModel.CurrentCodeBlockBuilder.Parent;
    	var outOwner = outBuilder?.CodeBlockOwner;
    	
    	// Update Scope
    	{
	    	var scope = compilationUnit.BinderSession.ScopeList[compilationUnit.BinderSession.CurrentScopeIndexKey];
	    	scope.EndingIndexExclusive = textSpan.EndingIndexExclusive;
	    	compilationUnit.BinderSession.ScopeList[compilationUnit.BinderSession.CurrentScopeIndexKey] = scope;
	    	
	    	// Restore Parent Scope
			if (scope.ParentIndexKey is not null)
			{
				compilationUnit.BinderSession.CurrentScopeIndexKey = scope.ParentIndexKey.Value;
			}
    	}
    	
    	// Update CodeBlockOwner
    	if (inOwner is not null)
    	{
	        inOwner.SetCodeBlockNode(inBuilder.Build(), parserModel.DiagnosticBag, parserModel.TokenWalker);
			
			if (inOwner.SyntaxKind == SyntaxKind.NamespaceStatementNode)
				compilationUnit.Binder.BindNamespaceStatementNode((NamespaceStatementNode)inOwner, compilationUnit);
			else if (inOwner.SyntaxKind == SyntaxKind.TypeDefinitionNode)
				compilationUnit.Binder.BindTypeDefinitionNode((TypeDefinitionNode)inOwner, compilationUnit, true);
			
			// Restore Parent CodeBlockBuilder
			if (outBuilder is not null)
			{
				parserModel.CurrentCodeBlockBuilder = outBuilder;
				outBuilder.InnerPendingCodeBlockOwner = null;
				
				if (inOwner.SyntaxKind != SyntaxKind.TryStatementTryNode &&
					inOwner.SyntaxKind != SyntaxKind.TryStatementCatchNode &&
					inOwner.SyntaxKind != SyntaxKind.TryStatementFinallyNode)
				{
					outBuilder.ChildList.Add(inOwner);
				}
			}
		}
    }

    public void BindTypeDefinitionNode(
        TypeDefinitionNode typeDefinitionNode,
        CSharpCompilationUnit compilationUnit,
        bool shouldOverwrite = false)
    {
        var typeIdentifierText = typeDefinitionNode.TypeIdentifierToken.TextSpan.GetText();
        var currentNamespaceStatementText = compilationUnit.BinderSession.CurrentNamespaceStatementNode.IdentifierToken.TextSpan.GetText();
        var namespaceAndTypeIdentifiers = new NamespaceAndTypeIdentifiers(currentNamespaceStatementText, typeIdentifierText);

        typeDefinitionNode.EncompassingNamespaceIdentifierString = currentNamespaceStatementText;
        
        if (TryGetTypeDefinitionNodeByScope(
        		compilationUnit,
        		compilationUnit.BinderSession.ResourceUri,
        		compilationUnit.BinderSession.CurrentScopeIndexKey,
        		typeIdentifierText,
        		out var existingTypeDefinitionNode))
        {
        	if (shouldOverwrite || existingTypeDefinitionNode.IsFabricated)
        	{
        		SetTypeDefinitionNodeByScope(
        			compilationUnit,
        			compilationUnit.BinderSession.ResourceUri,
	        		compilationUnit.BinderSession.CurrentScopeIndexKey,
	        		typeIdentifierText,
	        		typeDefinitionNode);
        	}
        }
        else
        {
        	_ = TryAddTypeDefinitionNodeByScope(
        		compilationUnit,
    			compilationUnit.BinderSession.ResourceUri,
        		compilationUnit.BinderSession.CurrentScopeIndexKey,
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
        CSharpCompilationUnit compilationUnit)
    {
        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            symbol.TextSpan.GetText(),
            compilationUnit.BinderSession.CurrentScopeIndexKey);

        var symbolDefinition = new SymbolDefinition(
            compilationUnit.BinderSession.CurrentScopeIndexKey,
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

        AddSymbolReference(symbol, compilationUnit);
    }

    private void AddSymbolReference(ISymbol symbol, CSharpCompilationUnit compilationUnit)
    {
        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            symbol.TextSpan.GetText(),
            compilationUnit.BinderSession.CurrentScopeIndexKey);

        if (!_symbolDefinitions.TryGetValue(
                symbolDefinitionId,
                out var symbolDefinition))
        {
            symbolDefinition = new SymbolDefinition(
                compilationUnit.BinderSession.CurrentScopeIndexKey,
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
            compilationUnit.BinderSession.CurrentScopeIndexKey));
    }

	/// <summary>
	/// Returns the 'symbolId: compilationUnit.BinderSession.GetNextSymbolId();'
	/// that was used to construct the ITextEditorSymbol.
	/// </summary>
    public int CreateVariableSymbol(
        IdentifierToken identifierToken,
        VariableKind variableKind,
        CSharpCompilationUnit compilationUnit)
    {
    	var symbolId = compilationUnit.BinderSession.GetNextSymbolId();
    	
        switch (variableKind)
        {
            case VariableKind.Field:
                AddSymbolDefinition(
                	new FieldSymbol(
	                	symbolId,
	                	identifierToken.TextSpan with
		                {
		                    DecorationByte = (byte)GenericDecorationKind.Field
		                }),
		            compilationUnit);
                break;
            case VariableKind.Property:
                AddSymbolDefinition(
                	new PropertySymbol(
                		symbolId,
                		identifierToken.TextSpan with
		                {
		                    DecorationByte = (byte)GenericDecorationKind.Property
		                }),
	                compilationUnit);
                break;
            case VariableKind.Local:
                goto default;
            case VariableKind.Closure:
                goto default;
            default:
                AddSymbolDefinition(
                	new VariableSymbol(
                		symbolId,
                		identifierToken.TextSpan with
		                {
		                    DecorationByte = (byte)GenericDecorationKind.Variable
		                }),
		            compilationUnit);
                break;
        }
        
        return symbolId;
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
                .ToList();

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
    	CSharpCompilationUnit? compilationUnit,
        ResourceUri resourceUri,
    	int initialScopeIndexKey,
        string identifierText,
        out FunctionDefinitionNode? functionDefinitionNode)
    {
        var localScope = GetScopeByScopeIndexKey(compilationUnit, resourceUri, initialScopeIndexKey);

        while (localScope is not null)
        {
            if (TryGetFunctionDefinitionNodeByScope(
	        		compilationUnit,
            		resourceUri,
            		localScope.IndexKey,
            		identifierText,
                    out functionDefinitionNode))
            {
                return true;
            }

			if (localScope.ParentIndexKey is null)
				localScope = null;
			else
            	localScope = GetScopeByScopeIndexKey(compilationUnit, resourceUri, localScope.ParentIndexKey.Value);
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
    	CSharpCompilationUnit? compilationUnit,
        ResourceUri resourceUri,
    	int initialScopeIndexKey,
        string identifierText,
        out TypeDefinitionNode? typeDefinitionNode)
    {
        var localScope = GetScopeByScopeIndexKey(compilationUnit, resourceUri, initialScopeIndexKey);

        while (localScope is not null)
        {
            if (TryGetTypeDefinitionNodeByScope(
	        		compilationUnit,
            		resourceUri,
            		localScope.IndexKey,
            		identifierText,
                    out typeDefinitionNode))
            {
                return true;
            }

            if (localScope.ParentIndexKey is null)
				localScope = null;
			else
            	localScope = GetScopeByScopeIndexKey(compilationUnit, resourceUri, localScope.ParentIndexKey.Value);
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
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int initialScopeIndexKey,
        string identifierText,
        out IVariableDeclarationNode? variableDeclarationStatementNode)
    {
        var localScope = GetScopeByScopeIndexKey(compilationUnit, resourceUri, initialScopeIndexKey);

        while (localScope is not null)
        {
            if (TryGetVariableDeclarationNodeByScope(
	        		compilationUnit,
            		resourceUri,
            		localScope.IndexKey,
            		identifierText,
                    out variableDeclarationStatementNode))
            {
                return true;
            }

            if (localScope.ParentIndexKey is null)
				localScope = null;
			else
            	localScope = GetScopeByScopeIndexKey(compilationUnit, resourceUri, localScope.ParentIndexKey.Value);
        }

        variableDeclarationStatementNode = null;
        return false;
    }

    IScope? IBinder.GetScope(TextEditorTextSpan textSpan) =>
    	GetScope(compilationUnit: null, textSpan);
    	
    public IScope? GetScope(CSharpCompilationUnit? compilationUnit, TextEditorTextSpan textSpan)
    {
    	return GetScopeByPositionIndex(compilationUnit, textSpan.ResourceUri, textSpan.StartingIndexInclusive);
    }
    
    IScope? IBinder.GetScopeByPositionIndex(ResourceUri resourceUri, int positionIndex) =>
    	GetScopeByPositionIndex(compilationUnit: null, resourceUri, positionIndex);
    
    public IScope? GetScopeByPositionIndex(CSharpCompilationUnit? compilationUnit, ResourceUri resourceUri, int positionIndex)
    {
    	var scopeList = new List<IScope>();
    	
    	if (TryGetBinderSession(compilationUnit, resourceUri, out var targetBinderSession))
    		scopeList.AddRange(targetBinderSession.ScopeList);
		if (TryGetBinderSession(compilationUnit, ResourceUri.Empty, out var globalBinderSession))
    		scopeList.AddRange(globalBinderSession.ScopeList);
        
        var possibleScopes = scopeList.Where(x =>
        {
            return x.StartingIndexInclusive <= positionIndex &&
            	   // Global Scope awkwardly has a null ending index exclusive (2023-10-15)
                   (x.EndingIndexExclusive >= positionIndex || x.EndingIndexExclusive is null);
        });

        return possibleScopes.MinBy(x => positionIndex - x.StartingIndexInclusive);
    }
    
    IScope? IBinder.GetScopeByScopeIndexKey(ResourceUri resourceUri, int scopeIndexKey) =>
    	GetScopeByScopeIndexKey(compilationUnit: null, resourceUri, scopeIndexKey);
    
    public IScope? GetScopeByScopeIndexKey(CSharpCompilationUnit? compilationUnit, ResourceUri resourceUri, int scopeIndexKey)
    {
    	var scopeList = new List<IScope>();
    	
    	if (TryGetBinderSession(compilationUnit, resourceUri, out var targetBinderSession))
    		scopeList.AddRange(targetBinderSession.ScopeList);
		if (TryGetBinderSession(compilationUnit, ResourceUri.Empty, out var globalBinderSession))
    		scopeList.AddRange(globalBinderSession.ScopeList);
        
        return scopeList[scopeIndexKey];
    }
    
    IScope[]? IBinder.GetScopeList(ResourceUri resourceUri) =>
    	GetScopeList(compilationUnit: null, resourceUri);
    
    public IScope[]? GetScopeList(CSharpCompilationUnit? compilationUnit, ResourceUri resourceUri)
    {
    	var scopeList = new List<IScope>();
    
    	if (TryGetBinderSession(compilationUnit, resourceUri, out var targetBinderSession))
    		scopeList.AddRange(targetBinderSession.ScopeList);
		if (TryGetBinderSession(compilationUnit, ResourceUri.Empty, out var globalBinderSession))
    		scopeList.AddRange(globalBinderSession.ScopeList);
    		
    	return scopeList.ToArray();
    }
    
    bool IBinder.TryGetBinderSession(ResourceUri resourceUri, out IBinderSession binderSession) =>
    	TryGetBinderSession(compilationUnit: null, resourceUri, out binderSession);
    
    /// <summary>
    /// If the resourceUri is the in progress BinderSession's ResourceUri,
    /// then the in progress instance should be returned via the out variable.
    ///
    /// TODO: This is quite confusingly written at the moment. 
    /// </summary>
    public bool TryGetBinderSession(CSharpCompilationUnit? compilationUnit, ResourceUri resourceUri, out IBinderSession binderSession)
    {
    	if (compilationUnit is not null &&
    		resourceUri == compilationUnit.BinderSession.ResourceUri)
    	{
    		binderSession = compilationUnit.BinderSession;
    		return true;
    	}
    	
    	var success = _binderSessionMap.TryGetValue(resourceUri, out var x);
    	binderSession = x;
    	return success;
    }
    
    void IBinder.UpsertBinderSession(IBinderSession binderSession) =>
    	UpsertBinderSession((CSharpBinderSession)binderSession);
    
    public void UpsertBinderSession(CSharpBinderSession binderSession)
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
    
    TypeDefinitionNode[] IBinder.GetTypeDefinitionNodesByScope(ResourceUri resourceUri, int scopeIndexKey) =>
    	GetTypeDefinitionNodesByScope(compilationUnit: null, resourceUri, scopeIndexKey);
    
    public TypeDefinitionNode[] GetTypeDefinitionNodesByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    		return Array.Empty<TypeDefinitionNode>();
    	
    	return binderSession.ScopeTypeDefinitionMap
    		.Where(kvp => kvp.Key.ScopeIndexKey == scopeIndexKey)
    		.Select(kvp => kvp.Value)
    		.ToArray();
    }
    
    bool IBinder.TryGetTypeDefinitionNodeByScope(ResourceUri resourceUri, int scopeIndexKey, string typeIdentifierText, out TypeDefinitionNode typeDefinitionNode) =>
    	TryGetTypeDefinitionNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey, typeIdentifierText, out typeDefinitionNode);
    
    public bool TryGetTypeDefinitionNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string typeIdentifierText,
    	out TypeDefinitionNode typeDefinitionNode)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    	{
    		typeDefinitionNode = null;
    		return false;
    	}
    	
    	var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeIndexKey, typeIdentifierText);
    	return binderSession.ScopeTypeDefinitionMap.TryGetValue(scopeKeyAndIdentifierText, out typeDefinitionNode);
    }
    
    bool IBinder.TryAddTypeDefinitionNodeByScope(ResourceUri resourceUri, int scopeIndexKey, string typeIdentifierText, TypeDefinitionNode typeDefinitionNode) =>
    	TryAddTypeDefinitionNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey, typeIdentifierText, typeDefinitionNode);
        
    public bool TryAddTypeDefinitionNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    		return false;
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeIndexKey, typeIdentifierText);
    	return binderSession.ScopeTypeDefinitionMap.TryAdd(scopeKeyAndIdentifierText, typeDefinitionNode);
    }
    
    void IBinder.SetTypeDefinitionNodeByScope(ResourceUri resourceUri, int scopeIndexKey, string typeIdentifierText, TypeDefinitionNode typeDefinitionNode) =>
    	SetTypeDefinitionNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey, typeIdentifierText, typeDefinitionNode);
    
    public void SetTypeDefinitionNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string typeIdentifierText,
        TypeDefinitionNode typeDefinitionNode)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    		return;

		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeIndexKey, typeIdentifierText);
    	binderSession.ScopeTypeDefinitionMap[scopeKeyAndIdentifierText] = typeDefinitionNode;
    }
    
    FunctionDefinitionNode[] IBinder.GetFunctionDefinitionNodesByScope(ResourceUri resourceUri, int scopeIndexKey) =>
    	GetFunctionDefinitionNodesByScope(compilationUnit: null, resourceUri, scopeIndexKey);
    
    public FunctionDefinitionNode[] GetFunctionDefinitionNodesByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    		return Array.Empty<FunctionDefinitionNode>();

    	return binderSession.ScopeFunctionDefinitionMap
    		.Where(kvp => kvp.Key.ScopeIndexKey == scopeIndexKey)
    		.Select(kvp => kvp.Value)
    		.ToArray();
    }
    
    bool IBinder.TryGetFunctionDefinitionNodeByScope(ResourceUri resourceUri, int scopeIndexKey, string functionIdentifierText, out FunctionDefinitionNode functionDefinitionNode) =>
    	TryGetFunctionDefinitionNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey, functionIdentifierText, out functionDefinitionNode);
    
    public bool TryGetFunctionDefinitionNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string functionIdentifierText,
    	out FunctionDefinitionNode functionDefinitionNode)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    	{
    		functionDefinitionNode = null;
    		return false;
    	}
    		
    	var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeIndexKey, functionIdentifierText);
    	return binderSession.ScopeFunctionDefinitionMap.TryGetValue(scopeKeyAndIdentifierText, out functionDefinitionNode);
    }
    
    bool IBinder.TryAddFunctionDefinitionNodeByScope(ResourceUri resourceUri, int scopeIndexKey, string functionIdentifierText, FunctionDefinitionNode functionDefinitionNode) =>
    	TryAddFunctionDefinitionNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey, functionIdentifierText, functionDefinitionNode);
    
    public bool TryAddFunctionDefinitionNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    		return false;
    	
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeIndexKey, functionIdentifierText);
    	return binderSession.ScopeFunctionDefinitionMap.TryAdd(scopeKeyAndIdentifierText, functionDefinitionNode);
    }

	void IBinder.SetFunctionDefinitionNodeByScope(ResourceUri resourceUri, int scopeIndexKey, string functionIdentifierText, FunctionDefinitionNode functionDefinitionNode) =>
		SetFunctionDefinitionNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey, functionIdentifierText, functionDefinitionNode);

    public void SetFunctionDefinitionNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string functionIdentifierText,
        FunctionDefinitionNode functionDefinitionNode)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    		return;
    	
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeIndexKey, functionIdentifierText);
    	binderSession.ScopeFunctionDefinitionMap[scopeKeyAndIdentifierText] = functionDefinitionNode;
    }
    
    IVariableDeclarationNode[] IBinder.GetVariableDeclarationNodesByScope(ResourceUri resourceUri, int scopeIndexKey) =>
    	GetVariableDeclarationNodesByScope(compilationUnit: null, resourceUri, scopeIndexKey);

    public IVariableDeclarationNode[] GetVariableDeclarationNodesByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    		return Array.Empty<IVariableDeclarationNode>();
    	
    	return binderSession.ScopeVariableDeclarationMap
    		.Where(kvp => kvp.Key.ScopeIndexKey == scopeIndexKey)
    		.Select(kvp => kvp.Value)
    		.ToArray();
    }
    
    bool IBinder.TryGetVariableDeclarationNodeByScope(ResourceUri resourceUri, int scopeIndexKey, string variableIdentifierText, out IVariableDeclarationNode variableDeclarationNode) =>
    	TryGetVariableDeclarationNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey, variableIdentifierText, out variableDeclarationNode);
    
    public bool TryGetVariableDeclarationNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
    	out IVariableDeclarationNode variableDeclarationNode)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    	{
    		variableDeclarationNode = null;
    		return false;
    	}
    		
    	var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeIndexKey, variableIdentifierText);
    	return binderSession.ScopeVariableDeclarationMap.TryGetValue(scopeKeyAndIdentifierText, out variableDeclarationNode);
    }
    
    bool IBinder.TryAddVariableDeclarationNodeByScope(ResourceUri resourceUri, int scopeIndexKey, string variableIdentifierText, IVariableDeclarationNode variableDeclarationNode) =>
    	TryAddVariableDeclarationNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey, variableIdentifierText, variableDeclarationNode);
        
    public bool TryAddVariableDeclarationNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    		return false;
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeIndexKey, variableIdentifierText);
    	return binderSession.ScopeVariableDeclarationMap.TryAdd(scopeKeyAndIdentifierText, variableDeclarationNode);
    }
    
    void IBinder.SetVariableDeclarationNodeByScope(ResourceUri resourceUri, int scopeIndexKey, string variableIdentifierText, IVariableDeclarationNode variableDeclarationNode) =>
    	SetVariableDeclarationNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey, variableIdentifierText, variableDeclarationNode);
        
    public void SetVariableDeclarationNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey,
    	string variableIdentifierText,
        IVariableDeclarationNode variableDeclarationNode)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    		return;
    		
		var scopeKeyAndIdentifierText = new ScopeKeyAndIdentifierText(scopeIndexKey, variableIdentifierText);
    	binderSession.ScopeVariableDeclarationMap[scopeKeyAndIdentifierText] = variableDeclarationNode;
    }

	TypeClauseNode? IBinder.GetReturnTypeClauseNodeByScope(ResourceUri resourceUri, int scopeIndexKey) =>
		GetReturnTypeClauseNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey);

    public TypeClauseNode? GetReturnTypeClauseNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey)
    {
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    		return null;
    	
    	if (binderSession.ScopeReturnTypeClauseNodeMap.TryGetValue(scopeIndexKey, out var returnTypeClauseNode))
    		return returnTypeClauseNode;
    	else
    		return null;
    }
    
    bool IBinder.TryAddReturnTypeClauseNodeByScope(ResourceUri resourceUri, int scopeIndexKey, TypeClauseNode typeClauseNode) =>
    	TryAddReturnTypeClauseNodeByScope(compilationUnit: null, resourceUri, scopeIndexKey, typeClauseNode);
    
    public bool TryAddReturnTypeClauseNodeByScope(
    	CSharpCompilationUnit? compilationUnit,
    	ResourceUri resourceUri,
    	int scopeIndexKey,
        TypeClauseNode typeClauseNode)
	{    	
    	if (!TryGetBinderSession(compilationUnit, resourceUri, out var binderSession))
    	{
    		typeClauseNode = null;
    		return false;
    	}
    		
    	return binderSession.ScopeReturnTypeClauseNodeMap.TryAdd(scopeIndexKey, typeClauseNode);
    }
    
    public TextEditorTextSpan? GetDefinitionTextSpan(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource) =>
    	GetDefinitionTextSpan(compilationUnit: null, textSpan, compilerServiceResource);
    	
    public TextEditorTextSpan? GetDefinitionTextSpan(CSharpCompilationUnit? compilationUnit, TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource)
    {
    	var symbol = GetSymbol(compilationUnit, textSpan, compilerServiceResource.GetSymbols());
        var definitionNode = GetDefinitionNode(compilationUnit, textSpan, symbol.SyntaxKind);
        
        if (definitionNode is null)
        	return null;
        
        switch (definitionNode.SyntaxKind)
        {
        	case SyntaxKind.VariableDeclarationNode:
        		return ((VariableDeclarationNode)definitionNode).IdentifierToken.TextSpan;
        	case SyntaxKind.FunctionDefinitionNode:
        		return ((FunctionDefinitionNode)definitionNode).FunctionIdentifierToken.TextSpan;
        	case SyntaxKind.TypeDefinitionNode:
	        	return ((TypeDefinitionNode)definitionNode).TypeIdentifierToken.TextSpan;
	        default:
	        	return null;
        }
    }
    
    public ITextEditorSymbol GetSymbol(CSharpCompilationUnit? compilationUnit, TextEditorTextSpan textSpan, IReadOnlyList<ITextEditorSymbol> symbolList)
    {
    	// Try to find a symbol at that cursor position.
		var foundSymbol = (ITextEditorSymbol?)null;
		
        foreach (var symbol in symbolList)
        {
            if (textSpan.StartingIndexInclusive >= symbol.TextSpan.StartingIndexInclusive &&
                textSpan.StartingIndexInclusive < symbol.TextSpan.EndingIndexExclusive)
            {
                foundSymbol = symbol;
                break;
            }
        }
		
		return foundSymbol;
    }
    
    ISyntaxNode? IBinder.GetDefinitionNode(TextEditorTextSpan textSpan, ICompilerServiceResource compilerServiceResource, ITextEditorSymbol? symbol = null)
    {
    	symbol ??= GetSymbol(compilationUnit: null, textSpan, compilerServiceResource.GetSymbols());
    	if (symbol is null)
    		return null;
    		
    	return GetDefinitionNode(compilationUnit: null, textSpan, symbol.SyntaxKind, symbol: symbol);
    }
    
    /// <summary>
    /// If the 'syntaxKind' is unknown then a possible way of determining it is to invoke <see cref="GetSymbol"/>
    /// and use the symbol's syntaxKind.
    /// </summary>
    public ISyntaxNode? GetDefinitionNode(CSharpCompilationUnit? compilationUnit, TextEditorTextSpan textSpan, SyntaxKind syntaxKind, ITextEditorSymbol? symbol = null)
    {
    	Console.WriteLine("aaa aaaaaaa");
    
    	var scope = GetScope(compilationUnit, textSpan);
        
        switch (syntaxKind)
        {
        	case SyntaxKind.VariableAssignmentExpressionNode:
        	case SyntaxKind.VariableDeclarationNode:
        	case SyntaxKind.VariableReferenceNode:
        	case SyntaxKind.VariableSymbol:
        	case SyntaxKind.PropertySymbol:
        	case SyntaxKind.FieldSymbol:
        	{
        		if (TryGetVariableDeclarationHierarchically(
        				compilationUnit,
        				textSpan.ResourceUri,
        				scope.IndexKey,
		                textSpan.GetText(),
		                out var variableDeclarationStatementNode)
		            && variableDeclarationStatementNode is not null)
		        {
		            return variableDeclarationStatementNode;
		        }
		        
		        Console.WriteLine("aaa check symbol is not null");
		        // Console.WriteLine($"aaa target: {((CSharpBinderSession)targetBinderSession).SymbolIdToExternalTextSpanMap.Count}");
			    // Console.WriteLine($"aaa other: {compilationUnit.BinderSession.SymbolIdToExternalTextSpanMap.Count}");
		        if (symbol is not null)
		        {
		        	Console.WriteLine("aaa yes symbol is not null");
			        if (TryGetBinderSession(compilationUnit, textSpan.ResourceUri, out var targetBinderSession))
			        {
			        	Console.WriteLine($"aaa target: {((CSharpBinderSession)targetBinderSession).SymbolIdToExternalTextSpanMap.Count}");
			        	// Console.WriteLine($"aaa other: {compilationUnit.BinderSession.SymbolIdToExternalTextSpanMap.Count}");
			        
			        	Console.WriteLine("zzzzaaa adasdasdasasdasdyes symbol is not null");
			        	
			        	if (((CSharpBinderSession)targetBinderSession).SymbolIdToExternalTextSpanMap.TryGetValue(symbol.SymbolId, out var definitionTextSpan))
			        	{
			        		Console.WriteLine("aaa found em");
			        	
			        		/*var definitionSymbol = GetSymbol(compilationUnit, definitionTextSpan, compilerServiceResource.GetSymbols());
					    	if (symbol is null)
					    		return null;*/
    		
			        		return GetDefinitionNode(compilationUnit, definitionTextSpan, SyntaxKind.VariableDeclarationNode);
			        	}
			        }
		        }
		        
		        return null;
        	}
        	case SyntaxKind.FunctionInvocationNode:
        	case SyntaxKind.FunctionDefinitionNode:
        	case SyntaxKind.FunctionSymbol:
	        {
	        	if (TryGetFunctionHierarchically(
	        				 compilationUnit,
	        				 textSpan.ResourceUri,
        					 scope.IndexKey,
		                     textSpan.GetText(),
		                     out var functionDefinitionNode)
		                 && functionDefinitionNode is not null)
		        {
		            return functionDefinitionNode;
		        }
		        
		        return null;
	        }
	        case SyntaxKind.TypeClauseNode:
	        case SyntaxKind.TypeDefinitionNode:
	        case SyntaxKind.TypeSymbol:
	        case SyntaxKind.ConstructorSymbol:
	        {
	        	if (TryGetTypeDefinitionHierarchically(
	        				 compilationUnit,
	        			     textSpan.ResourceUri,
        					 scope.IndexKey,
		                     textSpan.GetText(),
		                     out var typeDefinitionNode)
		                 && typeDefinitionNode is not null)
		        {
		            return typeDefinitionNode;
		        }
		        
		        return null;
	        }
        }

        return null;
    }

    ISyntaxNode? IBinder.GetSyntaxNode(int positionIndex, ResourceUri resourceUri, ICompilerServiceResource? compilerServiceResource) =>
    	GetSyntaxNode(compilationUnit: null, positionIndex, resourceUri, compilerServiceResource);
    
    public ISyntaxNode? GetSyntaxNode(CSharpCompilationUnit? compilationUnit, int positionIndex, ResourceUri resourceUri, ICompilerServiceResource? compilerServiceResource)
    {
        var scope = GetScopeByPositionIndex(compilationUnit, resourceUri, positionIndex);
        if (scope is null)
        	return null;
        
        ISyntaxNode parentNode;
        	
        var codeBlockOwner = scope.CodeBlockOwner;
        
        if (codeBlockOwner is not null)
        	parentNode = (ISyntaxNode)codeBlockOwner.CodeBlockNode;
        else if (compilerServiceResource.CompilationUnit is not null)
        	parentNode = compilerServiceResource.CompilationUnit.RootCodeBlockNode;
        else
        	return null;
        
        if (parentNode is null)
        	return null;
        
        var childList = parentNode.GetChildList();
        var possibleNodeList = new List<ISyntaxNode>();
        
        ISyntaxNode? fallbackDefinitionNode = null;
        
        foreach (var child in childList)
        {
        	if (child is not ISyntaxNode node)
    			continue;
    			
    		if (node.SyntaxKind == SyntaxKind.FunctionDefinitionNode ||
    			node.SyntaxKind == SyntaxKind.ConstructorDefinitionNode)
    		{
    			fallbackDefinitionNode = node;
    		}
        
        	var nodePositionIndices = GetNodePositionIndices(node);
        	if (nodePositionIndices == (-1, -1))
        		continue;
        		
        	if (nodePositionIndices.StartInclusiveIndex <= positionIndex &&
        		nodePositionIndices.EndExclusiveIndex >= positionIndex)
        	{
        		possibleNodeList.Add(node);
        	}
        }
        
        if (possibleNodeList.Count <= 0)
        {
        	if (fallbackDefinitionNode is not null)
        	{
        		if (fallbackDefinitionNode.SyntaxKind == SyntaxKind.FunctionDefinitionNode ||
        			fallbackDefinitionNode.SyntaxKind == SyntaxKind.ConstructorDefinitionNode)
        		{
        			var fallbackCodeBlockOwner = ((ICodeBlockOwner)fallbackDefinitionNode);
        			TextEditorTextSpan? fallbackTextSpan = null;
        			
        			if (fallbackCodeBlockOwner.OpenBraceToken.ConstructorWasInvoked)
        				fallbackTextSpan = fallbackCodeBlockOwner.OpenBraceToken.TextSpan;
        			else if (fallbackCodeBlockOwner.StatementDelimiterToken.ConstructorWasInvoked)
        				fallbackTextSpan = fallbackCodeBlockOwner.StatementDelimiterToken.TextSpan;
        				
        			if (fallbackTextSpan is not null && compilerServiceResource is not null)
        			{
        				var fallbackScope = GetScopeByPositionIndex(compilationUnit, resourceUri, fallbackTextSpan.Value.StartingIndexInclusive);
        				if (scope is not null)
        					return GetFallbackNode(compilationUnit, positionIndex, resourceUri, compilerServiceResource, fallbackScope);
        			}
        		}
        	}
        	
        	return null;
        }
        	
        return possibleNodeList.MinBy(node =>
        {
        	// TODO: Wasteful re-invocation of this method, can probably do this in one invocation.
        	var nodePositionIndices = GetNodePositionIndices(node);
        	if (nodePositionIndices == (-1, -1))
        		return int.MaxValue;
        	
        	return positionIndex - nodePositionIndices.StartInclusiveIndex;
        });
    }
    
    /// <summary>
    /// TODO: In 'GetDefinitionNode(...)' The positionIndex to determine IScope is the same that is used to determine the 'name' of the ISyntaxNode...
    /// 	  ...This should likely be changed, because function argument goto definition won't work if done from the argument listing, rather than the code block of the function.
    /// 	  This method will act as a temporary work around.
    /// </summary>
    public ISyntaxNode? GetFallbackNode(CSharpCompilationUnit? compilationUnit, int positionIndex, ResourceUri resourceUri, ICompilerServiceResource compilerServiceResource, IScope scope)
    {
        if (compilerServiceResource.CompilationUnit is null)
        	return null;
        
        // Try to find a symbol at that cursor position.
		var symbols = compilerServiceResource.GetSymbols();
		var foundSymbol = (ITextEditorSymbol?)null;
		
        foreach (var symbol in symbols)
        {
            if (positionIndex >= symbol.TextSpan.StartingIndexInclusive &&
                positionIndex < symbol.TextSpan.EndingIndexExclusive)
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
        				compilationUnit,
        				foundSymbol.TextSpan.ResourceUri,
        				scope.IndexKey,
		                foundSymbol.TextSpan.GetText(),
		                out var variableDeclarationStatementNode)
		            && variableDeclarationStatementNode is not null)
		        {
		            return variableDeclarationStatementNode;
		        }
		        
		        return null;
        	}
        }

        return null;
    }
    
    /// <summary>
    /// If the provided syntaxNode's SyntaxKind is not recognized, then (-1, -1) is returned.
    ///
    /// Otherwise, this method is meant to understand all of the ISyntaxToken
    /// that the node encompasses.
    ///
    /// With this knowledge, the method can determine the ISyntaxToken that starts, and ends the node
    /// within the source code.
    ///
    /// Then, it returns the indices from the start and end token.
    ///
    /// The ISyntaxNode instances are in a large enough count that it was decided not
    /// to make this an instance method on each ISyntaxNode.
    ///
    /// ========================================================================
    /// There is no overhead per-object-instance for adding a method to a class.
    /// https://stackoverflow.com/a/48861218/14847452
    /// 
    /// 	"Yes, C#/.Net methods require memory on per-AppDomain basis, there is no per-instance cost of the methods/properties.
	/// 	
	/// 	Cost comes from:
	/// 	
	/// 	methods metadata (part of type) and IL. I'm not sure how long IL stays loaded as it really only needed to JIT so my guess it is loaded as needed and discarded.
	/// 	after method is JITed machine code stays till AppDomain is unloaded (or if compiled as neutral till process terminates)
	/// 	So instantiating 1 or 50 objects with 50 methods will not require different amount of memory for methods."
    /// ========================================================================
    ///
    /// But, while there is no overhead to having this be on each implementation of 'ISyntaxNode',
    /// it is believed to still belong in the IBinder.
    ///
    /// This is because each language needs to have control over the various nodes.
    /// As one node in C# is not necessarily read the same as it would be by a python 'ICompilerService'.
    ///
    /// The goal with the ISyntaxNode implementations seems to be:
    /// - Keep them as generalized as possible.
    /// - Any specific details should be provided by the IBinder.
    /// </summary>
    public (int StartInclusiveIndex, int EndExclusiveIndex) GetNodePositionIndices(ISyntaxNode syntaxNode)
    {
    	switch (syntaxNode.SyntaxKind)
    	{
    		case SyntaxKind.TypeDefinitionNode:
    		{
    			var typeDefinitionNode = (TypeDefinitionNode)syntaxNode;
    			
    			if (typeDefinitionNode.TypeIdentifierToken.ConstructorWasInvoked)
    				return (typeDefinitionNode.TypeIdentifierToken.TextSpan.StartingIndexInclusive, typeDefinitionNode.TypeIdentifierToken.TextSpan.EndingIndexExclusive);
    			
    			goto default;
    		}
    		case SyntaxKind.FunctionDefinitionNode:
    		{
    			var functionDefinitionNode = (FunctionDefinitionNode)syntaxNode;
    			
    			if (functionDefinitionNode.FunctionIdentifierToken.ConstructorWasInvoked)
    				return (functionDefinitionNode.FunctionIdentifierToken.TextSpan.StartingIndexInclusive, functionDefinitionNode.FunctionIdentifierToken.TextSpan.EndingIndexExclusive);
    			
    			goto default;
    		}
    		case SyntaxKind.ConstructorDefinitionNode:
    		{
    			var constructorDefinitionNode = (ConstructorDefinitionNode)syntaxNode;
    			
    			if (constructorDefinitionNode.FunctionIdentifier.ConstructorWasInvoked)
    				return (constructorDefinitionNode.FunctionIdentifier.TextSpan.StartingIndexInclusive, constructorDefinitionNode.FunctionIdentifier.TextSpan.EndingIndexExclusive);
    			
    			goto default;
    		}
    		case SyntaxKind.VariableDeclarationNode:
    		{
    			var variableDeclarationNode = (VariableDeclarationNode)syntaxNode;
    			
    			if (variableDeclarationNode.IdentifierToken.ConstructorWasInvoked)
    				return (variableDeclarationNode.IdentifierToken.TextSpan.StartingIndexInclusive, variableDeclarationNode.IdentifierToken.TextSpan.EndingIndexExclusive);
    			
    			goto default;
    		}
    		default:
    		{
    			#if DEBUG
    			Console.WriteLine($"method: '{nameof(GetNodePositionIndices)}' The {nameof(SyntaxKind)}: '{syntaxNode}' defaulted in switch statement.");
    			#endif
    			
    			return (-1, -1);
    		}
    	}
    }
    
    public void OnBoundScopeCreatedAndSetAsCurrent(ICodeBlockOwner codeBlockOwner, CSharpCompilationUnit compilationUnit, ref CSharpParserModel parserModel)
    {
    	if (codeBlockOwner.SyntaxKind == SyntaxKind.TypeDefinitionNode)
    	{
    		var typeDefinitionNode = (TypeDefinitionNode)codeBlockOwner;
    		
    		if (typeDefinitionNode.PrimaryConstructorFunctionArgumentsListingNode is not null)
	    	{
	    		foreach (var argument in typeDefinitionNode.PrimaryConstructorFunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
		    	{
		    		compilationUnit.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, compilationUnit);
		    	}
	    	}
    	}
    	else if (codeBlockOwner.SyntaxKind == SyntaxKind.NamespaceStatementNode)
    	{
    		var namespaceStatementNode = (NamespaceStatementNode)codeBlockOwner;
    		var namespaceString = namespaceStatementNode.IdentifierToken.TextSpan.GetText();
        	compilationUnit.Binder.AddNamespaceToCurrentScope(namespaceString, compilationUnit);
    	}
    	else if (codeBlockOwner.SyntaxKind == SyntaxKind.FunctionDefinitionNode)
    	{
    		var functionDefinitionNode = (FunctionDefinitionNode)codeBlockOwner;
    		foreach (var argument in functionDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
	    	{
	    		compilationUnit.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, compilationUnit);
	    	}
    	}
    	else if (codeBlockOwner.SyntaxKind == SyntaxKind.ForeachStatementNode)
    	{
    		var foreachStatementNode = (ForeachStatementNode)codeBlockOwner;
    		compilationUnit.Binder.BindVariableDeclarationNode(foreachStatementNode.VariableDeclarationNode, compilationUnit);
    	}
    	else if (codeBlockOwner.SyntaxKind == SyntaxKind.ConstructorDefinitionNode)
    	{
    		var constructorDefinitionNode = (ConstructorDefinitionNode)codeBlockOwner;
    		foreach (var argument in constructorDefinitionNode.FunctionArgumentsListingNode.FunctionArgumentEntryNodeList)
			{
				compilationUnit.Binder.BindVariableDeclarationNode(argument.VariableDeclarationNode, compilationUnit);
			}
    	}
    	else if (codeBlockOwner.SyntaxKind == SyntaxKind.LambdaExpressionNode)
    	{
    		var lambdaExpressionNode = (LambdaExpressionNode)codeBlockOwner;
    		foreach (var variableDeclarationNode in lambdaExpressionNode.VariableDeclarationNodeList)
	    	{
	    		compilationUnit.Binder.BindVariableDeclarationNode(variableDeclarationNode, compilationUnit);
	    	}
    	}
    	else if (codeBlockOwner.SyntaxKind == SyntaxKind.TryStatementCatchNode)
    	{
    		var tryStatementCatchNode = (TryStatementCatchNode)codeBlockOwner;
    		
    		if (tryStatementCatchNode.VariableDeclarationNode is not null)
	    		compilationUnit.Binder.BindVariableDeclarationNode(tryStatementCatchNode.VariableDeclarationNode, compilationUnit);
    	}
    }
}
