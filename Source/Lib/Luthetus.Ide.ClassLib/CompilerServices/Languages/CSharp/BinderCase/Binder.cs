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
    private readonly Dictionary<string, BoundNamespaceStatementNode> _boundNamespaceStatementNodes = new();
    private readonly List<ISymbol> _symbols = new();
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

    private string _sourceText;
    private List<BoundScope> _boundScopes = new();
    private BoundScope _currentScope;

    public Binder(
        string sourceText)
    {
        _sourceText = sourceText;
        _currentScope = _globalScope;

        _boundScopes.Add(_globalScope);

        _boundScopes = _boundScopes
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();
    }

    public ImmutableDictionary<string, BoundNamespaceStatementNode> BoundNamespaceStatementNodes => _boundNamespaceStatementNodes.ToImmutableDictionary();
    public ImmutableArray<ISymbol> Symbols => _symbols.ToImmutableArray();
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

    public bool TryBindTypeNode(
        ISyntaxToken token,
        out BoundTypeNode? boundTypeNode)
    {
        var text = token.TextSpan.GetText(_sourceText);

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
        var functionIdentifier = identifierToken.TextSpan.GetText(_sourceText);

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
            identifierToken);

        _currentScope.FunctionDeclarationMap.Add(
            functionIdentifier,
            boundFunctionDeclarationNode);

        _symbols.Add(
            new FunctionSymbol(identifierToken.TextSpan with
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
    
    public BoundNamespaceStatementNode BindNamespaceStatementNode(
        KeywordToken keywordToken,
        IdentifierToken identifierToken)
    {
        var namespaceIdentifier = identifierToken.TextSpan.GetText(_sourceText);

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

            _boundNamespaceStatementNodes.Add(
                namespaceIdentifier,
                boundNamespaceStatementNode);

            return boundNamespaceStatementNode;
        }
    }
    
    public BoundNamespaceStatementNode RegisterBoundNamespaceEntryNode(
        BoundNamespaceStatementNode inBoundNamespaceStatementNode,
        CompilationUnit compilationUnit)
    {
        var namespaceIdentifier = inBoundNamespaceStatementNode
            .IdentifierToken.TextSpan.GetText(_sourceText);

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
        var classIdentifier = identifierToken.TextSpan.GetText(_sourceText);

        if (_currentScope.ClassDeclarationMap.TryGetValue(
            classIdentifier,
            out var classDeclarationNode))
        {
            // TODO: The class was already declared, so report a diagnostic?
            // TODO: The class was already declared, so check that the return types match?
            return classDeclarationNode;
        }

        var boundClassDeclarationNode = new BoundClassDeclarationNode(
            identifierToken);

        _currentScope.ClassDeclarationMap.Add(
            classIdentifier,
            boundClassDeclarationNode);

        _symbols.Add(
            new TypeSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Type
            }));

        return boundClassDeclarationNode;
    }
    
    public BoundInheritanceStatementNode BindInheritanceStatementNode(
        IdentifierToken parentClassIdentifierToken)
    {
        var parentClassIdentifier = parentClassIdentifierToken.TextSpan.GetText(_sourceText);

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
        var text = identifierToken.TextSpan.GetText(_sourceText);

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
            identifierToken);

        _currentScope.VariableDeclarationMap.Add(
            text,
            boundVariableDeclarationStatementNode);

        return boundVariableDeclarationStatementNode;
    }

    /// <summary>Returns null if the variable was not yet declared.</summary>
    public BoundVariableAssignmentStatementNode? BindVariableAssignmentNode(
        IdentifierToken identifierToken,
        IBoundExpressionNode boundExpressionNode)
    {
        var text = identifierToken.TextSpan.GetText(_sourceText);

        if (TryGetVariableHierarchically(
                text,
                out var variableDeclarationNode) &&
            variableDeclarationNode is not null)
        {
            if (variableDeclarationNode.IsInitialized)
                return new(identifierToken, boundExpressionNode);

            variableDeclarationNode = variableDeclarationNode
                .WithIsInitialized(true);

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

    public BoundFunctionInvocationNode? BindFunctionInvocationNode(
        IdentifierToken identifierToken)
    {
        var text = identifierToken.TextSpan.GetText(_sourceText);

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

    public void RegisterBoundScope(
        Type? scopeReturnType,
        TextEditorTextSpan textEditorTextSpan)
    {
        var boundScope = new BoundScope(
            _currentScope,
            scopeReturnType,
            textEditorTextSpan.StartingIndexInclusive,
            null,
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

    public void DisposeBoundScope(
        TextEditorTextSpan textEditorTextSpan)
    {
        if (_currentScope.Parent is not null)
        {
            _currentScope.EndingIndexExclusive = textEditorTextSpan.EndingIndexExclusive;
            _currentScope = _currentScope.Parent;
        }
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

    public void SetSourceText(
        string sourceText)
    {
        _sourceText = sourceText;
    }
}
