﻿using System.Collections.Immutable;
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
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.Facts;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.C.BinderCase;

public class CBinderSession
{
    private readonly BoundScope _globalScope = CLanguageFacts.Scope.GetInitialGlobalScope();
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

    private BoundScope _currentScope;

    public CBinderSession()
    {
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

    public bool TryBindClassReferenceNode(
        ISyntaxToken typeClauseToken,
        BoundGenericArgumentsNode? boundGenericArgumentsNode,
        out BoundClassReferenceNode? boundClassReferenceNode)
    {
        var text = typeClauseToken.TextSpan.GetText();

        if (_currentScope.ClassDefinitionMap.TryGetValue(text, out _))
        {
            boundClassReferenceNode = new BoundClassReferenceNode(
                typeClauseToken,
                // TODO: Don't pass null here
                null,
                boundGenericArgumentsNode);

            return true;
        }

        boundClassReferenceNode = null;
        return false;
    }

    public BoundFunctionDefinitionNode BindFunctionDefinitionNode(
        BoundClassReferenceNode boundClassReferenceNode,
        IdentifierToken identifierToken,
        BoundFunctionArgumentsNode boundFunctionArguments,
        BoundGenericArgumentsNode? boundGenericArgumentsNode)
    {
        var text = identifierToken.TextSpan.GetText();

        if (_currentScope.FunctionDefinitionMap.TryGetValue(
            text,
            out var functionDefinitionNode))
        {
            // TODO: The function was already declared, so report a diagnostic?
            // TODO: The function was already declared, so check that the return types match?
            return functionDefinitionNode;
        }

        var boundFunctionDefinitionNode = new BoundFunctionDefinitionNode(
            boundClassReferenceNode,
            identifierToken,
            boundFunctionArguments,
            boundGenericArgumentsNode,
            null);

        _currentScope.FunctionDefinitionMap.Add(
            text,
            boundFunctionDefinitionNode);

        Symbols.Add(
            new FunctionSymbol(identifierToken.TextSpan with
            {
                DecorationByte = (byte)GenericDecorationKind.Function
            }));

        return boundFunctionDefinitionNode;
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

    public BoundVariableDeclarationStatementNode BindVariableDeclarationNode(
        BoundClassReferenceNode boundClassReferenceNode,
        IdentifierToken identifierToken)
    {
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
            boundClassReferenceNode,
            identifierToken,
            false);

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

    public BoundFunctionInvocationNode? BindFunctionInvocationNode(
        IdentifierToken identifierToken,
        BoundFunctionParametersNode boundFunctionParametersNode)
    {
        var text = identifierToken.TextSpan.GetText();

        if (TryGetBoundFunctionDefinitionNodeHierarchically(
                text,
                out var boundFunctionDefinitionNode) &&
            boundFunctionDefinitionNode is not null)
        {
            return new(identifierToken, boundFunctionParametersNode, null);
        }
        else
        {
            _diagnosticBag.ReportUndefinedFunction(
                identifierToken.TextSpan,
                text);

            return new(identifierToken, boundFunctionParametersNode, null)
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
            textEditorTextSpan.ResourceUri,
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
    public bool TryGetBoundFunctionDefinitionNodeHierarchically(
        string text,
        out BoundFunctionDefinitionNode? boundFunctionDefinitionNode)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.FunctionDefinitionMap.TryGetValue(
                    text,
                    out boundFunctionDefinitionNode))
            {
                return true;
            }

            localScope = localScope.Parent;
        }

        boundFunctionDefinitionNode = null;
        return false;
    }

    /// <summary>Search hierarchically through all the scopes, starting at the <see cref="_currentScope"/>.<br/><br/>If a match is found, then set the out parameter to it and return true.<br/><br/>If none of the searched scopes contained a match then set the out parameter to null and return false.</summary>
    public bool TryGetClassHierarchically(
        ISyntaxToken typeClauseToken,
        BoundGenericArgumentsNode? boundGenericArgumentsNode,
        out BoundClassReferenceNode? boundClassReferenceNode)
    {
        var localScope = _currentScope;

        while (localScope is not null)
        {
            if (localScope.ClassDefinitionMap.TryGetValue(
                    typeClauseToken.TextSpan.GetText(),
                    out _))
            {
                boundClassReferenceNode = new BoundClassReferenceNode(
                    typeClauseToken,
                    // TODO: Don't pass null here
                    null,
                    boundGenericArgumentsNode);

                return true;
            }

            localScope = localScope.Parent;
        }

        boundClassReferenceNode = null;
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
