using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Symbols;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.Facts;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.BinderCase;

public class Binder
{
    private readonly BoundScope _globalScope = CSharpLanguageFacts.Scope.GetInitialGlobalScope();
    private readonly Dictionary<string, BoundNamespaceStatementNode> _boundNamespaceStatementNodes = CSharpLanguageFacts.Namespaces.GetInitialBoundNamespaceStatementNodes();
    /// <summary>The key for _symbolDefinitions is calculated by <see cref="ISymbol.GetSymbolDefinitionId"/></summary>
    private readonly Dictionary<string, SymbolDefinition> _symbolDefinitions = new();
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

    private List<BoundScope> _boundScopes = new();
    private BoundScope _currentScope;

    public Binder()
    {
        _currentScope = _globalScope;

        _boundScopes.Add(_globalScope);

        _boundScopes = _boundScopes
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();
    }

    public ImmutableDictionary<string, BoundNamespaceStatementNode> BoundNamespaceStatementNodes => _boundNamespaceStatementNodes.ToImmutableDictionary();
    public ImmutableArray<ISymbol> Symbols => _symbolDefinitions.Values.SelectMany(x => x.SymbolReferences).Select(x => x.Symbol).ToImmutableArray();
    public Dictionary<string, SymbolDefinition> SymbolDefinitions => _symbolDefinitions;
    public ImmutableArray<BoundScope> BoundScopes => _boundScopes.ToImmutableArray();
    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();

    public BoundLiteralExpressionNode BindLiteralExpressionNode(
        LiteralExpressionNode literalExpressionNode)
    {
        var type = literalExpressionNode.LiteralSyntaxToken.SyntaxKind switch
        {
            SyntaxKind.NumericLiteralToken => typeof(int),
            SyntaxKind.StringLiteralToken => typeof(string),
            _ => throw new NotImplementedException(),
        };

        var boundLiteralExpressionNode = new BoundLiteralExpressionNode(
            literalExpressionNode.LiteralSyntaxToken,
            type);

        return boundLiteralExpressionNode;
    }

    public BoundBinaryOperatorNode BindBinaryOperatorNode(
        BoundLiteralExpressionNode leftBoundLiteralExpressionNode,
        ISyntaxToken operatorToken,
        BoundLiteralExpressionNode rightBoundLiteralExpressionNode)
    {
        if (leftBoundLiteralExpressionNode.ResultType == typeof(int) &&
            rightBoundLiteralExpressionNode.ResultType == typeof(int))
        {
            switch (operatorToken.SyntaxKind)
            {
                case SyntaxKind.PlusToken:
                    return new BoundBinaryOperatorNode(
                        leftBoundLiteralExpressionNode.ResultType,
                        operatorToken,
                        rightBoundLiteralExpressionNode.ResultType,
                        typeof(int));
            }
        }
        else if (leftBoundLiteralExpressionNode.ResultType == typeof(string) &&
            rightBoundLiteralExpressionNode.ResultType == typeof(string))
        {
            switch (operatorToken.SyntaxKind)
            {
                case SyntaxKind.PlusToken:
                    return new BoundBinaryOperatorNode(
                        leftBoundLiteralExpressionNode.ResultType,
                        operatorToken,
                        rightBoundLiteralExpressionNode.ResultType,
                        typeof(string));
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

    public bool TryBindTypeNode(
        ISyntaxToken token,
        out BoundTypeNode? boundTypeNode)
    {
        var text = token.TextSpan.GetText();

        if (_currentScope.TypeMap.TryGetValue(text, out var type))
        {
            boundTypeNode = new BoundTypeNode(type, token);
            return true;
        }

        boundTypeNode = null;
        return false;
    }

    public BoundFunctionDeclarationNode BindFunctionDeclarationNode(
        BoundTypeNode boundTypeNode,
        IdentifierToken identifierToken)
    {
        var functionIdentifier = identifierToken.TextSpan.GetText();

        if (_currentScope.FunctionDeclarationMap.TryGetValue(
            functionIdentifier,
            out var functionDeclarationNode))
        {
            // TODO: The function was already declared, so report a diagnostic?
            // TODO: The function was already declared, so check that the return types match?
            return functionDeclarationNode;
        }

        var boundFunctionDeclarationNode = new BoundFunctionDeclarationNode(
            boundTypeNode,
            identifierToken,
            null);

        var success = _currentScope.FunctionDeclarationMap.TryAdd(
            functionIdentifier,
            boundFunctionDeclarationNode);

        if (!success)
            _currentScope.FunctionDeclarationMap[functionIdentifier] = boundFunctionDeclarationNode;

        AddSymbolDefinition(new FunctionSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Function
        }));

        return boundFunctionDeclarationNode;
    }

    /// <summary>TODO: Validate that the returned bound expression node has the same result type as the enclosing scope.</summary>
    public BoundReturnStatementNode BindReturnStatementNode(
        KeywordToken keywordToken,
        IBoundExpressionNode boundExpressionNode)
    {
        _diagnosticBag.ReportReturnStatementsAreStillBeingImplemented(
                keywordToken.TextSpan);

        return new BoundReturnStatementNode(
            keywordToken,
            boundExpressionNode);
    }

    public BoundIfStatementNode BindIfStatementNode(
        KeywordToken ifKeywordToken,
        IBoundExpressionNode boundExpressionNode)
    {
        var boundIfStatementNode = new BoundIfStatementNode(
            ifKeywordToken,
            boundExpressionNode,
            null);

        return boundIfStatementNode;
    }

    public BoundNamespaceStatementNode BindNamespaceStatementNode(
        KeywordToken keywordToken,
        IdentifierToken identifierToken)
    {
        var namespaceIdentifier = identifierToken.TextSpan.GetText();

        if (_boundNamespaceStatementNodes.TryGetValue(
                namespaceIdentifier, 
                out var boundNamespaceStatementNode))
        {
            return boundNamespaceStatementNode;
        }
        else
        {
            boundNamespaceStatementNode = new BoundNamespaceStatementNode(
                keywordToken,
                identifierToken,
                ImmutableArray<CompilationUnit>.Empty);

            var success = _boundNamespaceStatementNodes.TryAdd(
                namespaceIdentifier,
                boundNamespaceStatementNode);

            if (!success)
                _boundNamespaceStatementNodes[namespaceIdentifier] = boundNamespaceStatementNode;

            return boundNamespaceStatementNode;
        }
    }
    
    public BoundNamespaceStatementNode RegisterBoundNamespaceEntryNode(
        BoundNamespaceStatementNode inBoundNamespaceStatementNode,
        CompilationUnit compilationUnit)
    {
        var namespaceIdentifier = inBoundNamespaceStatementNode
            .IdentifierToken.TextSpan.GetText();

        if (_boundNamespaceStatementNodes.TryGetValue(
                namespaceIdentifier, 
                out var existingBoundNamespaceStatementNode))
        {
            var outChildren = existingBoundNamespaceStatementNode.Children
                .Add(compilationUnit)
                .Select(x => (CompilationUnit)x)
                .ToImmutableArray();

            var outBoundNamespaceStatementNode = new BoundNamespaceStatementNode(
                existingBoundNamespaceStatementNode.KeywordToken,
                existingBoundNamespaceStatementNode.IdentifierToken,
                outChildren);

            _boundNamespaceStatementNodes[namespaceIdentifier] = outBoundNamespaceStatementNode;

            return outBoundNamespaceStatementNode;
        }
        else
        {
            throw new NotImplementedException(
                $"The {nameof(inBoundNamespaceStatementNode)}" +
                $" was not found in the {nameof(_boundNamespaceStatementNodes)} dictionary.");
        }
    }

    public BoundClassDeclarationNode BindClassDeclarationNode(
        IdentifierToken identifierToken)
    {
        var classIdentifier = identifierToken.TextSpan.GetText();

        if (_currentScope.ClassDeclarationMap.TryGetValue(
            classIdentifier,
            out var classDeclarationNode))
        {
            // TODO: The class was already declared, so report a diagnostic?
            // TODO: The class was already declared, so check that the return types match?
            return classDeclarationNode;
        }

        var boundClassDeclarationNode = new BoundClassDeclarationNode(
            identifierToken,
            null,
            null);

        var success = _currentScope.ClassDeclarationMap.TryAdd(
            classIdentifier,
            boundClassDeclarationNode);

        if (!success)
            _currentScope.ClassDeclarationMap[classIdentifier] = boundClassDeclarationNode;

        AddSymbolDefinition(new TypeSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type
        }));

        return boundClassDeclarationNode;
    }
    
    public BoundConstructorInvocationNode BindConstructorInvocationNode(
        KeywordToken keywordToken,
        BoundTypeNode? boundTypeNode,
        BoundFunctionArgumentsNode boundFunctionArgumentsNode,
        BoundObjectInitializationNode? boundObjectInitializationNode)
    {
        return new BoundConstructorInvocationNode(
            keywordToken,
            boundTypeNode,
            boundFunctionArgumentsNode,
            boundObjectInitializationNode);
    }
    
    public BoundInheritanceStatementNode BindInheritanceStatementNode(
        IdentifierToken parentClassIdentifierToken)
    {
        AddSymbolReference(new TypeSymbol(parentClassIdentifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type
        }));

        var parentClassIdentifier = parentClassIdentifierToken.TextSpan.GetText();

        var boundInheritanceStatementNode = new BoundInheritanceStatementNode(
                parentClassIdentifierToken);

        if (!_currentScope.ClassDeclarationMap.TryGetValue(
            parentClassIdentifier,
            out _))
        {
            _diagnosticBag.ReportUndefinedClass(
                parentClassIdentifierToken.TextSpan,
                parentClassIdentifier);
        }

        return boundInheritanceStatementNode;
    }

    public BoundVariableDeclarationStatementNode BindVariableDeclarationNode(
        BoundTypeNode boundTypeNode,
        IdentifierToken identifierToken)
    {
        AddSymbolDefinition(new VariableSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Variable
        }));
        
        var text = identifierToken.TextSpan.GetText();

        if (_currentScope.VariableDeclarationMap.TryGetValue(
            text,
            out var variableDeclarationNode))
        {
            // TODO: The variable was already declared, so report a diagnostic?
            // TODO: The variable was already declared, so check that the return types match?
            return variableDeclarationNode;
        }

        var boundVariableDeclarationStatementNode = new BoundVariableDeclarationStatementNode(
            boundTypeNode,
            identifierToken,
            false);

        var success = _currentScope.VariableDeclarationMap.TryAdd(
            text,
            boundVariableDeclarationStatementNode);

        if (!success)
            _currentScope.VariableDeclarationMap[text] = boundVariableDeclarationStatementNode;

        return boundVariableDeclarationStatementNode;
    }

    /// <summary>Returns null if the variable was not yet declared.</summary>
    public BoundVariableAssignmentStatementNode? BindVariableAssignmentNode(
        IdentifierToken identifierToken,
        IBoundExpressionNode boundExpressionNode)
    {
        AddSymbolReference(new VariableSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Variable
        }));

        var text = identifierToken.TextSpan.GetText();

        if (TryGetVariableHierarchically(
                text,
                out var variableDeclarationNode) &&
            variableDeclarationNode is not null)
        {
            if (variableDeclarationNode.IsInitialized)
                return new(identifierToken, boundExpressionNode);

            variableDeclarationNode = variableDeclarationNode with
            {
                IsInitialized = true
            };

            _currentScope.VariableDeclarationMap[text] =
                variableDeclarationNode;

            return new(identifierToken, boundExpressionNode);
        }
        else
        {
            // TODO: The variable was not yet declared, so report a diagnostic?
            return null;
        }
    }

    public BoundVariableDeclarationStatementNode BindPropertyDeclarationNode(
        BoundTypeNode boundTypeNode,
        IdentifierToken identifierToken)
    {
        AddSymbolDefinition(new PropertySymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Property
        }));

        var text = identifierToken.TextSpan.GetText();

        if (_currentScope.VariableDeclarationMap.TryGetValue(
            text,
            out var propertyDeclarationNode))
        {
            // TODO: The property was already declared, so report a diagnostic?
            // TODO: The property was already declared, so check that the return types match?
            return propertyDeclarationNode;
        }

        var boundVariableDeclarationStatementNode = new BoundVariableDeclarationStatementNode(
            boundTypeNode,
            identifierToken,
            false);

        var success = _currentScope.VariableDeclarationMap.TryAdd(
            text,
            boundVariableDeclarationStatementNode);

        if (!success)
            _currentScope.VariableDeclarationMap[text] = boundVariableDeclarationStatementNode;

        return boundVariableDeclarationStatementNode;
    }

    public BoundIdentifierReferenceNode BindIdentifierReferenceNode(
        IdentifierToken identifierToken)
    {
        var text = identifierToken.TextSpan.GetText();

        if (TryGetVariableHierarchically(
                text,
                out var variableDeclarationNode) &&
            variableDeclarationNode is not null)
        {
            AddSymbolReference(new VariableSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Variable
            }));

            return new BoundIdentifierReferenceNode(
                identifierToken,
                variableDeclarationNode.BoundTypeNode.Type);
        }
        else if (TryGetTypeHierarchically(
                     text,
                     out var type) &&
                 type is not null)
        {
            AddSymbolReference(new TypeSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            }));

            return new BoundIdentifierReferenceNode(
                identifierToken,
                type);
        }
        else if (TryGetBoundFunctionDeclarationNodeHierarchically(
                     text,
                     out var boundFunctionDeclarationNode) &&
                 boundFunctionDeclarationNode is not null)
        {
            // TODO: Would this conditional branch be for method groups? @onclick="MethodName"

            AddSymbolReference(new FunctionSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Function
            }));

            return new BoundIdentifierReferenceNode(
                identifierToken,
                typeof(void));
        }
        else
        {
            // TODO: The identifier was not found, so report a diagnostic?
            return new BoundIdentifierReferenceNode(
                identifierToken,
                typeof(void))
            {
                IsFabricated = true
            };
        }
    }

    public BoundFunctionInvocationNode? BindFunctionInvocationNode(
        IdentifierToken identifierToken)
    {
        AddSymbolReference(new FunctionSymbol(identifierToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Function
        }));

        var text = identifierToken.TextSpan.GetText();

        if (TryGetBoundFunctionDeclarationNodeHierarchically(
                text,
                out var boundFunctionDeclarationNode) &&
            boundFunctionDeclarationNode is not null)
        {
            return new(identifierToken);
        }
        else
        {
            _diagnosticBag.ReportUndefindFunction(
                identifierToken.TextSpan,
                text);

            return new(identifierToken)
            {
                IsFabricated = true
            };
        }
    }

    public BoundUsingDeclarationNode BindUsingDeclarationNode(
        KeywordToken usingKeywordToken,
        IdentifierToken namespaceIdentifierToken)
    {
        var namespaceText = namespaceIdentifierToken.TextSpan.GetText();

        if (_boundNamespaceStatementNodes.TryGetValue(
                namespaceText,
                out var boundNamespaceStatementNode))
        {
            AddNamespaceToCurrentScope(boundNamespaceStatementNode);
        }

        return new BoundUsingDeclarationNode(
            usingKeywordToken,
            namespaceIdentifierToken);
    }

    /// <summary>TODO: Correctly implement this method. For now going to skip until the attribute closing square bracket.</summary>
    public BoundAttributeNode BindAttributeNode(
        OpenSquareBracketToken openSquareBracketToken,
        CloseSquareBracketToken closeSquareBracketToken)
    {
        AddSymbolReference(new TypeSymbol(openSquareBracketToken.TextSpan with
        {
            DecorationByte = (byte)GenericDecorationKind.Type,
            EndingIndexExclusive = closeSquareBracketToken.TextSpan.EndingIndexExclusive
        }));

        return new BoundAttributeNode(
            openSquareBracketToken,
            closeSquareBracketToken);
    }
    
    public BoundGenericArgumentsNode BindGenericArguments(
        OpenAngleBracketToken openAngleBracketToken,
        List<ISyntaxToken> genericArgumentListing,
        CloseAngleBracketToken closeAngleBracketToken)
    {
        var boundGenericArgumentListing = new List<ISyntax>();

        for (var i = 0; i < genericArgumentListing.Count; i++)
        {
            ISyntax syntax;

            if (i % 2 == 1)
            {
                // CommaToken
                syntax = genericArgumentListing[i];
            }
            else
            {
                var identifierToken = genericArgumentListing[i];

                syntax = new BoundTypeNode(typeof(void), identifierToken);

                AddSymbolReference(new TypeSymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Type
                }));
            }

            boundGenericArgumentListing.Add(syntax);
        }

        return new BoundGenericArgumentsNode(
            openAngleBracketToken,
            boundGenericArgumentListing,
            closeAngleBracketToken);
    }
    
    public BoundFunctionArgumentsNode BindFunctionArguments(
        OpenParenthesisToken openParenthesisToken,
        List<ISyntaxToken> functionArgumentListing,
        CloseParenthesisToken closeParenthesisToken)
    {
        var boundGenericArgumentListing = new List<ISyntax>();

        // Alternate between reading type identifier (null), argument identifier (true), and a single comma (false)
        bool? shouldMatch = null;

        // The initialized value for 'seenBoundTypeNode' should never occur. I don't want to mark this variable as nullable however.
        BoundTypeNode seenBoundTypeNode = new BoundTypeNode(typeof(void), openParenthesisToken);

        for (var i = 0; i < functionArgumentListing.Count; i++)
        {
            ISyntax syntax;

            if (shouldMatch is null)
            {
                var identifierToken = functionArgumentListing[i];

                syntax = new BoundTypeNode(typeof(void), identifierToken);

                AddSymbolReference(new TypeSymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Type
                }));
            }
            else if (shouldMatch.Value)
            {
                var identifierToken = functionArgumentListing[i];

                syntax = new BoundVariableDeclarationStatementNode(seenBoundTypeNode, identifierToken, false);

                AddSymbolReference(new VariableSymbol(identifierToken.TextSpan with
                {
                    DecorationByte = (byte)GenericDecorationKind.Type
                }));
            }
            else
            {
                // CommaToken
                syntax = functionArgumentListing[i];
            }

            boundGenericArgumentListing.Add(syntax);
        }

        return new BoundFunctionArgumentsNode(
            openParenthesisToken,
            boundGenericArgumentListing,
            closeParenthesisToken);
    }

    public void RegisterBoundScope(
        Type? scopeReturnType,
        TextEditorTextSpan textEditorTextSpan)
    {
        var boundScope = new BoundScope(
            _currentScope,
            scopeReturnType,
            textEditorTextSpan.StartingIndexInclusive,
            null,
            textEditorTextSpan.ResourceUri,
            new(),
            new(),
            new(),
            new());

        _boundScopes.Add(boundScope);

        _boundScopes = _boundScopes
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();

        _currentScope = boundScope;
    }
    
    public void AddNamespaceToCurrentScope(
        BoundNamespaceStatementNode boundNamespaceStatementNode)
    {
        foreach (var namespaceEntry in boundNamespaceStatementNode.Children)
        {
            var compilationUnit = (CompilationUnit)namespaceEntry;

            foreach (var child in compilationUnit.Children)
            {
                if (child.SyntaxKind != SyntaxKind.BoundClassDeclarationNode)
                    continue;

                var boundClassDeclarationNode = (BoundClassDeclarationNode)child;

                var identifierText = boundClassDeclarationNode.IdentifierToken.TextSpan
                    .GetText();

                var success = _currentScope.ClassDeclarationMap.TryAdd(
                    identifierText,
                    boundClassDeclarationNode);

                if (!success)
                {
                    _currentScope.ClassDeclarationMap[identifierText] =
                        boundClassDeclarationNode;
                }
            }
        }
    }

    public void DisposeBoundScope(
        TextEditorTextSpan textEditorTextSpan)
    {
        _currentScope.EndingIndexExclusive = textEditorTextSpan.EndingIndexExclusive;

        if (_currentScope.Parent is not null)
            _currentScope = _currentScope.Parent;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
    public bool TryGetBoundFunctionDeclarationNodeHierarchically(
        string text,
        out BoundFunctionDeclarationNode? boundFunctionDeclarationNode)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.FunctionDeclarationMap.TryGetValue(
                    text,
                    out boundFunctionDeclarationNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        boundFunctionDeclarationNode = null;
        return false;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
    public bool TryGetTypeHierarchically(
        string text,
        out Type? type)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.TypeMap.TryGetValue(
                    text,
                    out type))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        type = null;
        return false;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
    public bool TryGetVariableHierarchically(
        string text,
        out BoundVariableDeclarationStatementNode? boundVariableDeclarationStatementNode)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.VariableDeclarationMap.TryGetValue(
                    text,
                    out boundVariableDeclarationStatementNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        boundVariableDeclarationStatementNode = null;
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
}
