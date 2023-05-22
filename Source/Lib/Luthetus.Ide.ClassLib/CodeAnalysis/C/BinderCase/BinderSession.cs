using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.Parsing.C.Facts;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CodeAnalysis;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Symbols;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxNodes.Expression;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase;

public class BinderSession
{
    private readonly BoundScope _globalScope = CLanguageFacts.Scope.GetInitialGlobalScope();
    private readonly string _sourceText;
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

    private BoundScope _currentScope;

    public BinderSession(
        string sourceText)
    {
        _sourceText = sourceText;
        _currentScope = _globalScope;

        BoundScopes.Add(_globalScope);

        BoundScopes = BoundScopes
            .OrderBy(x => x.StartingIndexInclusive)
            .ToList();
    }

    public List<BoundScope> BoundScopes { get; private set; } = new();
    public List<ISymbol> Symbols { get; private set; } = new();

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
        var text = token.TextEditorTextSpan.GetText(_sourceText);

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
        var text = identifierToken.TextEditorTextSpan.GetText(_sourceText);

        if (_currentScope.FunctionDeclarationMap.TryGetValue(
            text,
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
            text,
            boundFunctionDeclarationNode);

        Symbols.Add(
            new FunctionSymbol(identifierToken.TextEditorTextSpan with
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
                keywordToken.TextEditorTextSpan);

        return new BoundReturnStatementNode(
            keywordToken,
            boundExpressionNode);
    }

    public BoundVariableDeclarationStatementNode BindVariableDeclarationNode(
        BoundTypeNode boundTypeNode,
        IdentifierToken identifierToken)
    {
        var text = identifierToken.TextEditorTextSpan.GetText(_sourceText);

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
        var text = identifierToken.TextEditorTextSpan.GetText(_sourceText);

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
        var text = identifierToken.TextEditorTextSpan.GetText(_sourceText);

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
                identifierToken.TextEditorTextSpan,
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
            new());

        BoundScopes.Add(boundScope);

        BoundScopes = BoundScopes
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
}
