using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.CSharp.Facts;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.BinderCase;

public class CSharpBinder : IBinder
{
    private readonly CSharpBoundScope _globalScope = CSharpFacts.Scope.GetInitialGlobalScope();
    private readonly Dictionary<string, NamespaceGroupNode> _namespaceGroupNodeMap = CSharpFacts.Namespaces.GetInitialBoundNamespaceStatementNodes();
    /// <summary>The key for _symbolDefinitions is calculated by <see cref="ISymbol.GetSymbolDefinitionId"/></summary>
    private readonly Dictionary<string, SymbolDefinition> _symbolDefinitions = new();
    private readonly LuthetusDiagnosticBag _diagnosticBag = new();

    private List<CSharpBoundScope> _boundScopes = new();
    private CSharpBoundScope _currentScope;

    public CSharpBinder()
    {
        _currentScope = _globalScope;

        _boundScopes.Add(_globalScope);

        _boundScopes = _boundScopes
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();
    }

    public ResourceUri? CurrentResourceUri { get; set; }

    public ImmutableDictionary<string, NamespaceGroupNode> NamespaceGroupNodes => _namespaceGroupNodeMap.ToImmutableDictionary();
    public ImmutableArray<ISymbol> Symbols => _symbolDefinitions.Values.SelectMany(x => x.SymbolReferences).Select(x => x.Symbol).ToImmutableArray();
    public Dictionary<string, SymbolDefinition> SymbolDefinitions => _symbolDefinitions;
    public ImmutableArray<CSharpBoundScope> BoundScopes => _boundScopes.ToImmutableArray();
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList => _diagnosticBag.ToImmutableArray();

    ImmutableArray<ITextEditorSymbol> IBinder.SymbolsList => Symbols
        .Select(s => (ITextEditorSymbol)s)
        .ToImmutableArray();

    public LiteralExpressionNode BindLiteralExpressionNode(LiteralExpressionNode literalExpressionNode)
    {
        var typeClauseNode = literalExpressionNode.LiteralSyntaxToken.SyntaxKind switch
        {
            SyntaxKind.NumericLiteralToken => CSharpFacts.Types.Int.ToTypeClause(),
            SyntaxKind.StringLiteralToken => CSharpFacts.Types.String.ToTypeClause(),
            _ => throw new NotImplementedException(),
        };

        return new LiteralExpressionNode(
            literalExpressionNode.LiteralSyntaxToken,
            typeClauseNode);
    }

    public BinaryOperatorNode BindBinaryOperatorNode(
        IExpressionNode leftExpressionNode,
        ISyntaxToken operatorToken,
        IExpressionNode rightExpressionNode)
    {
        if (leftExpressionNode.ResultTypeClauseNode.ValueType == typeof(int) &&
            rightExpressionNode.ResultTypeClauseNode.ValueType == typeof(int))
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
        else if (leftExpressionNode.ResultTypeClauseNode.ValueType == typeof(string) &&
            rightExpressionNode.ResultTypeClauseNode.ValueType == typeof(string))
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

        throw new NotImplementedException();
    }

    /// <summary>TODO: Construct a BoundStringInterpolationExpressionNode and identify the expressions within the string literal. For now I am just making the dollar sign the same color as a string literal.</summary>
    public void BindStringInterpolationExpression(
        DollarSignToken dollarSignToken)
    {
        AddSymbolReference(new StringInterpolationSymbol(dollarSignToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.StringLiteral,
        }));
    }

    public void BindFunctionDefinitionNode(FunctionDefinitionNode functionDefinitionNode)
    {
        var functionIdentifierText = functionDefinitionNode.FunctionIdentifierToken.TextSpan.GetText();

        var functionSymbol = new FunctionSymbol(functionDefinitionNode.FunctionIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Function
        });

        AddSymbolDefinition(functionSymbol);

        if (!_currentScope.FunctionDefinitionMap.TryAdd(
                functionIdentifierText,
                functionDefinitionNode))
        {
            _diagnosticBag.ReportAlreadyDefinedFunction(
                functionDefinitionNode.FunctionIdentifierToken.TextSpan,
                functionIdentifierText);
        }
    }

    public FunctionArgumentEntryNode BindFunctionOptionalArgument(
        FunctionArgumentEntryNode functionArgumentEntryNode,
        ISyntaxToken compileTimeConstantToken,
        bool hasOutKeyword,
        bool hasInKeyword,
        bool hasRefKeyword)
    {
        var argumentTypeClauseNode = functionArgumentEntryNode.VariableDeclarationNode.TypeClauseNode;

        if (TryGetTypeDefinitionHierarchically(
                argumentTypeClauseNode.TypeIdentifierToken.TextSpan.GetText(),
                out var typeDefinitionNode)
            || typeDefinitionNode is null)
        {
            typeDefinitionNode = CSharpFacts.Types.Void;
        }

        var literalExpressionNode = new LiteralExpressionNode(
            compileTimeConstantToken,
            typeDefinitionNode.ToTypeClause());

        literalExpressionNode = BindLiteralExpressionNode(literalExpressionNode);

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

        return new FunctionArgumentEntryNode(
            functionArgumentEntryNode.VariableDeclarationNode,
            true,
            hasOutKeyword,
            hasInKeyword,
            hasRefKeyword);
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

    public void BindNamespaceStatementNode(NamespaceStatementNode namespaceStatementNode)
    {
        var namespaceString = namespaceStatementNode.IdentifierToken.TextSpan.GetText();
        AddSymbolReference(new NamespaceSymbol(namespaceStatementNode.IdentifierToken.TextSpan));

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

    public InheritanceStatementNode BindInheritanceStatementNode(TypeClauseNode typeClauseNode)
    {
        AddSymbolReference(new TypeSymbol(typeClauseNode.TypeIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type
        }));

        throw new NotImplementedException();
    }

    public void BindVariableDeclarationStatementNode(VariableDeclarationNode variableDeclarationNode)
    {
        CreateVariableSymbol(variableDeclarationNode.IdentifierToken, variableDeclarationNode.VariableKind);

        var text = variableDeclarationNode.IdentifierToken.TextSpan.GetText();

        if (!_currentScope.VariableDeclarationMap.TryAdd(
                text,
                variableDeclarationNode))
        {
            _diagnosticBag.ReportAlreadyDefinedVariable(
                variableDeclarationNode.IdentifierToken.TextSpan,
                text);
        }
    }

    public VariableReferenceNode BindVariableReferenceNode(VariableReferenceNode variableReferenceNode)
    {
        var text = variableReferenceNode.VariableIdentifierToken.TextSpan.GetText();
        VariableKind variableKind = VariableKind.Local;

        if (TryGetVariableDeclarationHierarchically(text, out var variableDeclarationNode)
            && variableDeclarationNode is not null)
        {
            variableReferenceNode = new VariableReferenceNode(
                variableReferenceNode.VariableIdentifierToken,
                variableDeclarationNode!);

            variableKind = variableDeclarationNode.VariableKind;
        }
        else
        {
            _diagnosticBag.ReportUndefinedVariable(
                variableReferenceNode.VariableIdentifierToken.TextSpan,
                text);
        }

        CreateVariableSymbol(variableReferenceNode.VariableIdentifierToken, variableKind);

        return variableReferenceNode;
    }

    public void BindVariableAssignmentExpressionNode(VariableAssignmentExpressionNode variableAssignmentExpressionNode)
    {
        var text = variableAssignmentExpressionNode.VariableIdentifierToken.TextSpan.GetText();
        VariableKind variableKind = VariableKind.Local;

        if (TryGetVariableDeclarationHierarchically(text, out var variableDeclarationNode)
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

        CreateVariableSymbol(variableAssignmentExpressionNode.VariableIdentifierToken, variableKind);
    }

    public void BindConstructorDefinitionIdentifierToken(
        IdentifierToken identifierToken)
    {
        var constructorSymbol = new ConstructorSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type
        });

        AddSymbolDefinition(constructorSymbol);
    }

    public void BindFunctionInvocationNode(FunctionInvocationNode functionInvocationNode)
    {
        var functionInvocationIdentifierText = functionInvocationNode
            .FunctionInvocationIdentifierToken.TextSpan.GetText();

        var functionSymbol = new FunctionSymbol(functionInvocationNode.FunctionInvocationIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Function
        });

        AddSymbolReference(functionSymbol);

        if (TryGetFunctionHierarchically(
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

    public void BindNamespaceReference(IdentifierToken namespaceIdentifierToken)
    {
        var namespaceSymbol = new NamespaceSymbol(namespaceIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.None
        });

        AddSymbolReference(namespaceSymbol);
    }

    public TypeClauseNode BindTypeClauseNode(TypeClauseNode typeClauseNode)
    {
        if (typeClauseNode.TypeIdentifierToken.SyntaxKind == SyntaxKind.IdentifierToken)
        {
            var typeSymbol = new TypeSymbol(typeClauseNode.TypeIdentifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            });

            AddSymbolReference(typeSymbol);
        }

        var matchingTypeDefintionNode = CSharpFacts.Types.TypeDefinitionNodes.SingleOrDefault(
            x => x.TypeIdentifier.TextSpan.GetText() == typeClauseNode.TypeIdentifierToken.TextSpan.GetText());

        if (matchingTypeDefintionNode is not null)
        {
            return new TypeClauseNode(
                typeClauseNode.TypeIdentifierToken,
                matchingTypeDefintionNode.ValueType,
                typeClauseNode.GenericParametersListingNode);
        }

        return typeClauseNode;
    }

    public void BindTypeIdentifier(IdentifierToken identifierToken)
    {
        if (identifierToken.SyntaxKind == SyntaxKind.IdentifierToken)
        {
            var typeSymbol = new TypeSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            });

            AddSymbolReference(typeSymbol);
        }
    }

    public UsingStatementNode BindUsingStatementNode(
        KeywordToken usingKeywordToken,
        IdentifierToken namespaceIdentifierToken)
    {
        AddSymbolReference(new NamespaceSymbol(namespaceIdentifierToken.TextSpan));

        AddNamespaceToCurrentScope(namespaceIdentifierToken.TextSpan.GetText());

        return new UsingStatementNode(
            usingKeywordToken,
            namespaceIdentifierToken);
    }

    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
    public AttributeNode BindAttributeNode(
        OpenSquareBracketToken openSquareBracketToken,
        List<ISyntaxToken> innerTokens,
        CloseSquareBracketToken closeSquareBracketToken)
    {
        AddSymbolReference(new TypeSymbol(openSquareBracketToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type,
            EndingIndexExclusive = closeSquareBracketToken.TextSpan.EndingIndexExclusive
        }));

        return new AttributeNode(
            openSquareBracketToken,
            innerTokens,
            closeSquareBracketToken);
    }

    public void RegisterBoundScope(
        TypeClauseNode? scopeReturnTypeClauseNode,
        TextEditorTextSpan textEditorTextSpan)
    {
        var boundScope = new CSharpBoundScope(
            _currentScope,
            scopeReturnTypeClauseNode,
            textEditorTextSpan.StartingIndexInclusive,
            null,
            textEditorTextSpan.ResourceUri,
            new(),
            new(),
            new());

        _boundScopes.Add(boundScope);

        _boundScopes = _boundScopes
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();

        _currentScope = boundScope;
    }

    public void AddNamespaceToCurrentScope(string namespaceString)
    {
        if (_namespaceGroupNodeMap.TryGetValue(namespaceString, out var namespaceGroupNode) &&
            namespaceGroupNode is not null)
        {
            var typeDefinitionNodes = namespaceGroupNode.GetTopLevelTypeDefinitionNodes();

            foreach (var typeDefinitionNode in typeDefinitionNodes)
            {
                BindTypeDefinitionNode(typeDefinitionNode);
            }
        }
    }

    public void DisposeBoundScope(TextEditorTextSpan textSpan)
    {
        _currentScope.EndingIndexExclusive = textSpan.EndingIndexExclusive;

        if (_currentScope.Parent is not null)
            _currentScope = _currentScope.Parent;
    }

    public IBoundScope? GetBoundScope(TextEditorTextSpan textSpan)
    {
        var possibleScopes = _boundScopes
            .Where(x => x.ResourceUri == textSpan.ResourceUri || x.ResourceUri.Value == string.Empty)
            .Where(x =>
            {
                return x.StartingIndexInclusive <= textSpan.StartingIndexInclusive &&
                       (x.EndingIndexExclusive is null || // Global Scope awkwardly has a null ending index exclusive (2023-10-15)
                            x.EndingIndexExclusive >= textSpan.StartingIndexInclusive);
            });

        return possibleScopes.MinBy(
            x => textSpan.StartingIndexInclusive - x.StartingIndexInclusive);
    }

    public TextEditorTextSpan? GetDefinition(TextEditorTextSpan textSpan)
    {
        var boundScope = GetBoundScope(textSpan) as CSharpBoundScope;

        if (TryGetVariableDeclarationHierarchically(
                textSpan.GetText(),
                out var variableDeclarationStatementNode,
                boundScope)
            && variableDeclarationStatementNode is not null)
        {
            return variableDeclarationStatementNode.IdentifierToken.TextSpan;
        }
        else if (TryGetFunctionHierarchically(
                     textSpan.GetText(),
                     out var functionDefinitionNode,
                     boundScope)
                 && functionDefinitionNode is not null)
        {
            return functionDefinitionNode.FunctionIdentifierToken.TextSpan;
        }
        else if (TryGetTypeDefinitionHierarchically(
                     textSpan.GetText(),
                     out var typeDefinitionNode,
                     boundScope)
                 && typeDefinitionNode is not null)
        {
            return typeDefinitionNode.TypeIdentifier.TextSpan;
        }

        return null;
    }

    /// <summary>
    /// Search hierarchically through all the scopes, starting at the <see cref="initialScope"/>.<br/><br/>
    /// If a match is found, then set the out parameter to it and return true.<br/><br/>
    /// If none of the searched scopes contained a match then set the out parameter to null and return false.
    /// </summary>
    public bool TryGetFunctionHierarchically(
        string text,
        out FunctionDefinitionNode? functionDefinitionNode,
        CSharpBoundScope? initialScope = null)
    {
        var localScope = initialScope ?? _currentScope;

        while (localScope is not null)
        {
            if (localScope.FunctionDefinitionMap.TryGetValue(
                    text,
                    out functionDefinitionNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        functionDefinitionNode = null;
        return false;
    }

    public void BindTypeDefinitionNode(
        TypeDefinitionNode typeDefinitionNode,
        bool shouldOverwrite = false)
    {
        var success = _currentScope.TypeDefinitionMap.TryAdd(
            typeDefinitionNode.TypeIdentifier.TextSpan.GetText(),
            typeDefinitionNode);

        if (!success && shouldOverwrite)
            _currentScope.TypeDefinitionMap[typeDefinitionNode.TypeIdentifier.TextSpan.GetText()] = typeDefinitionNode;
    }

    /// <summary>
    /// Search hierarchically through all the scopes, starting at the <see cref="initialScope"/>.<br/><br/>
    /// If a match is found, then set the out parameter to it and return true.<br/><br/>
    /// If none of the searched scopes contained a match then set the out parameter to null and return false.
    /// </summary>
    public bool TryGetTypeDefinitionHierarchically(
        string text,
        out TypeDefinitionNode? typeDefinitionNode,
        CSharpBoundScope? initialScope = null)
    {
        var localScope = initialScope ?? _currentScope;

        while (localScope is not null)
        {
            if (localScope.TypeDefinitionMap.TryGetValue(
                    text,
                    out typeDefinitionNode))
            {
                return true;
            }

            localScope = localScope.Parent;
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
        string text,
        out VariableDeclarationNode? variableDeclarationStatementNode,
        CSharpBoundScope? initialScope = null)
    {
        var localScope = initialScope ?? _currentScope;

        while (localScope is not null)
        {
            if (localScope.VariableDeclarationMap.TryGetValue(
                    text,
                    out variableDeclarationStatementNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        variableDeclarationStatementNode = null;
        return false;
    }

    /// <summary>This method will handle the <see cref="SymbolDefinition"/>, but also invoke <see cref="AddSymbolReference"/> because each definition is being treated as a reference itself.</summary>
    private void AddSymbolDefinition(ISymbol symbol)
    {
        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            symbol.TextSpan.GetText(),
            _currentScope.BoundScopeKey);

        var symbolDefinition = new SymbolDefinition(
            _currentScope.BoundScopeKey,
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

        AddSymbolReference(symbol);
    }

    private void AddSymbolReference(ISymbol symbol)
    {
        var symbolDefinitionId = ISymbol.GetSymbolDefinitionId(
            symbol.TextSpan.GetText(),
            _currentScope.BoundScopeKey);

        if (!_symbolDefinitions.TryGetValue(
                symbolDefinitionId,
                out var symbolDefinition))
        {
            symbolDefinition = new SymbolDefinition(
                _currentScope.BoundScopeKey,
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
            _currentScope.BoundScopeKey));
    }

    public void CreateVariableSymbol(IdentifierToken identifierToken, VariableKind variableKind)
    {
        switch (variableKind)
        {
            case VariableKind.Field:
                AddSymbolDefinition(new FieldSymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Field
                }));
                break;
            case VariableKind.Property:
                AddSymbolDefinition(new PropertySymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Property
                }));
                break;
            case VariableKind.Local:
                goto default;
            case VariableKind.Closure:
                goto default;
            default:
                AddSymbolDefinition(new VariableSymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Variable
                }));
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

        _boundScopes = _boundScopes
            .Where(x => x.ResourceUri != resourceUri)
            .ToList();

        _diagnosticBag.ClearByResourceUri(resourceUri);
    }
}