﻿using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Statement;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.BinderCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.C.ParserCase;

public class CParserSession
{
    private readonly TokenWalker _tokenWalker;
    private readonly CBinderSession _binder;
    private readonly CompilationUnitBuilder _globalCompilationUnitBuilder = new(null);
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();
    private readonly ImmutableArray<TextEditorDiagnostic> _lexerDiagnostics;

    public CParserSession(
        ImmutableArray<ISyntaxToken> tokens,
        ImmutableArray<TextEditorDiagnostic> lexerDiagnostics)
    {
        _lexerDiagnostics = lexerDiagnostics;
        _tokenWalker = new TokenWalker(tokens, _diagnosticBag);
        _binder = new CBinderSession();

        _currentCompilationUnitBuilder = _globalCompilationUnitBuilder;
    }

    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();
    public CBinderSession Binder => _binder;

    private ISyntaxNode? _nodeRecent;
    private CompilationUnitBuilder _currentCompilationUnitBuilder;

    /// <summary>When parsing the body of a function this is used in order to keep the function definition node itself in the syntax tree immutable.<br/><br/>That is to say, this action would create the function definition node and then append it.</summary>
    private Action<CompilationUnit>? _finalizeCompilationUnitAction;

    public CompilationUnit Parse()
    {
        while (true)
        {
            var consumedToken = _tokenWalker.Consume();

            switch (consumedToken.SyntaxKind)
            {
                case SyntaxKind.NumericLiteralToken:
                    ParseNumericLiteralToken((NumericLiteralToken)consumedToken);
                    break;
                case SyntaxKind.StringLiteralToken:
                    ParseStringLiteralToken((StringLiteralToken)consumedToken);
                    break;
                case SyntaxKind.PlusToken:
                    ParsePlusToken((PlusToken)consumedToken);
                    break;
                case SyntaxKind.PreprocessorDirectiveToken:
                    ParsePreprocessorDirectiveToken((PreprocessorDirectiveToken)consumedToken);
                    break;
                case SyntaxKind.CommentSingleLineToken:
                    // Do not parse comments.
                    break;
                case SyntaxKind.KeywordToken:
                    ParseKeywordToken((KeywordToken)consumedToken);
                    break;
                case SyntaxKind.IdentifierToken:
                    ParseIdentifierToken((IdentifierToken)consumedToken);
                    break;
                case SyntaxKind.OpenBraceToken:
                    ParseOpenBraceToken((OpenBraceToken)consumedToken);
                    break;
                case SyntaxKind.CloseBraceToken:
                    ParseCloseBraceToken((CloseBraceToken)consumedToken);
                    break;
                case SyntaxKind.StatementDelimiterToken:
                    ParseStatementDelimiterToken();
                    break;
                case SyntaxKind.EndOfFileToken:
                    if (_nodeRecent is IExpressionNode)
                    {
                        _currentCompilationUnitBuilder.IsExpression = true;
                        _currentCompilationUnitBuilder.Children.Add(_nodeRecent);
                    }
                    break;
            }

            if (consumedToken.SyntaxKind == SyntaxKind.EndOfFileToken)
                break;
        }

        return _currentCompilationUnitBuilder.Build(
            Diagnostics
                .Union(_binder.Diagnostics)
                .Union(_lexerDiagnostics)
                .ToImmutableArray());
    }

    private BoundLiteralExpressionNode ParseNumericLiteralToken(
        NumericLiteralToken inToken)
    {
        var literalExpressionNode = new LiteralExpressionNode(inToken);

        var boundLiteralExpressionNode = _binder
            .BindLiteralExpressionNode(literalExpressionNode);

        _nodeRecent = boundLiteralExpressionNode;

        return boundLiteralExpressionNode;
    }

    private BoundLiteralExpressionNode ParseStringLiteralToken(
        StringLiteralToken inToken)
    {
        var literalExpressionNode = new LiteralExpressionNode(inToken);

        var boundLiteralExpressionNode = _binder
                .BindLiteralExpressionNode(literalExpressionNode);

        _nodeRecent = boundLiteralExpressionNode;

        return boundLiteralExpressionNode;
    }

    private BoundBinaryExpressionNode ParsePlusToken(
        PlusToken inToken)
    {
        var localNodeCurrent = _nodeRecent;

        if (localNodeCurrent is not BoundLiteralExpressionNode leftBoundLiteralExpressionNode)
            throw new NotImplementedException();

        var nextToken = _tokenWalker.Consume();

        BoundLiteralExpressionNode rightBoundLiteralExpressionNode;

        if (nextToken.SyntaxKind == SyntaxKind.NumericLiteralToken)
        {
            rightBoundLiteralExpressionNode = ParseNumericLiteralToken(
                (NumericLiteralToken)nextToken);
        }
        else
        {
            rightBoundLiteralExpressionNode = ParseStringLiteralToken(
                (StringLiteralToken)nextToken);
        }

        var boundBinaryOperatorNode = _binder.BindBinaryOperatorNode(
            leftBoundLiteralExpressionNode,
            inToken,
            rightBoundLiteralExpressionNode);

        var boundBinaryExpressionNode = new BoundBinaryExpressionNode(
            leftBoundLiteralExpressionNode,
            boundBinaryOperatorNode,
            rightBoundLiteralExpressionNode);

        _nodeRecent = boundBinaryExpressionNode;

        return boundBinaryExpressionNode;
    }

    private IStatementNode ParsePreprocessorDirectiveToken(
        PreprocessorDirectiveToken inToken)
    {
        var nextToken = _tokenWalker.Consume();

        if (nextToken.SyntaxKind == SyntaxKind.LibraryReferenceToken)
        {
            var preprocessorLibraryReferenceStatement = new PreprocessorLibraryReferenceStatement(
                inToken,
                nextToken);

            _currentCompilationUnitBuilder.Children.Add(preprocessorLibraryReferenceStatement);

            return preprocessorLibraryReferenceStatement;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void ParseKeywordToken(
        KeywordToken inToken)
    {
        var text = inToken.TextSpan.GetText();

        if (_binder.TryGetClassHierarchically(inToken, null, out var boundClassDefinitionNode) &&
            boundClassDefinitionNode is not null)
        {
            // 'int', 'string', 'bool', etc...
            _nodeRecent = boundClassDefinitionNode;
        }
        else
        {
            // 'return', 'if', 'get', etc...

            if (text == "return")
            {
                // TODO: Make many keywords SyntaxKinds. Then if SyntaxKind.EndsWith("Keyword"); so that string checking doesn't need to be done.

                var boundReturnStatementNode = _binder.BindReturnStatementNode(
                    inToken,
                    ParseExpression());

                _currentCompilationUnitBuilder.Children.Add(boundReturnStatementNode);

                _nodeRecent = boundReturnStatementNode;
            }
            else
            {
                throw new NotImplementedException("Implement more keywords");
            }
        }
    }

    private void ParseIdentifierToken(
        IdentifierToken inToken)
    {
        var nextToken = _tokenWalker.Consume();

        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundClassReferenceNode)
        {
            // 'function definition' OR 'variable declaration' OR 'variable initialization'

            if (nextToken.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            {
                // 'function definition'

                var boundFunctionDefinitionNode = _binder.BindFunctionDefinitionNode(
                    (BoundClassReferenceNode)_nodeRecent,
                    inToken,
                    // TODO: I'm working on C# and breaking some C code. Need to look at this later.
                    null,
                    // TODO: I'm working on C# and breaking some C code. Need to look at this later.
                    null);

                _nodeRecent = boundFunctionDefinitionNode;

                ParseFunctionArguments();
            }
            else if (nextToken.SyntaxKind == SyntaxKind.EqualsToken ||
                     nextToken.SyntaxKind == SyntaxKind.StatementDelimiterToken)
            {
                // 'variable declaration' OR 'variable initialization'

                // 'variable declaration'
                var boundVariableDeclarationStatementNode = _binder.BindVariableDeclarationNode(
                    (BoundClassReferenceNode)_nodeRecent,
                    inToken);

                _currentCompilationUnitBuilder.Children.Add(boundVariableDeclarationStatementNode);

                if (nextToken.SyntaxKind == SyntaxKind.EqualsToken)
                {
                    // 'variable initialization'

                    var rightHandExpression = ParseExpression();

                    var boundVariableAssignmentNode = _binder.BindVariableAssignmentNode(
                        (IdentifierToken)boundVariableDeclarationStatementNode.IdentifierToken,
                        rightHandExpression);

                    if (boundVariableAssignmentNode is null)
                    {
                        // TODO: Why would boundVariableDeclarationStatementNode ever be null here? The variable had just been defined. I suppose what I mean to say is, should this get the '!' operator? The compiler is correctly complaining and the return type should have nullability in the case of undefined variables. So, use the not null operator?
                        throw new NotImplementedException();
                    }
                    else
                    {
                        _currentCompilationUnitBuilder.Children
                            .Add(boundVariableAssignmentNode);
                    }
                }

                var expectedStatementDelimiterToken = _tokenWalker.Consume();

                if (expectedStatementDelimiterToken.SyntaxKind != SyntaxKind.StatementDelimiterToken)
                    _ = _tokenWalker.Backtrack();

                _nodeRecent = null;
            }
            else
            {
                // TODO: Report a diagnostic
                throw new NotImplementedException();
            }
        }
        else
        {
            // 'function invocation' OR 'variable assignment' OR 'variable reference'

            if (nextToken.SyntaxKind == SyntaxKind.OpenParenthesisToken)
            {
                // 'function invocation'

                var boundFunctionInvocationNode = _binder.BindFunctionInvocationNode(
                    inToken,
                    // TODO: I'm working on C# and breaking this file, going to pass null for now
                    null);

                if (boundFunctionInvocationNode is null)
                    throw new ApplicationException($"{nameof(boundFunctionInvocationNode)} was null.");

                _currentCompilationUnitBuilder.Children.Add(boundFunctionInvocationNode);
            }
            else if (nextToken.SyntaxKind == SyntaxKind.EqualsToken)
            {
                // 'variable assignment'

                var rightHandExpression = ParseExpression();

                var boundVariableAssignmentNode = _binder.BindVariableAssignmentNode(
                    inToken,
                    rightHandExpression);

                if (boundVariableAssignmentNode is null)
                {
                    // TODO: Why would boundVariableDeclarationStatementNode ever be null here? The variable had just been defined. I suppose what I mean to say is, should this get the '!' operator? The compiler is correctly complaining and the return type should have nullability in the case of undefined variables. So, use the not null operator?
                    throw new NotImplementedException();
                }
                else
                {
                    _currentCompilationUnitBuilder.Children
                        .Add(boundVariableAssignmentNode);
                }
            }
            else
            {
                // 'variable reference'
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>TODO: Implement ParseFunctionArguments() correctly. Until then, skip until the body of the function is found. Specifically until the CloseParenthesisToken is found</summary>
    private void ParseFunctionArguments()
    {
        while (true)
        {
            var tokenCurrent = _tokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.CloseParenthesisToken)
            {
                break;
            }
        }
    }

    /// <summary>TODO: Implement ParseExpression() correctly. Until then, skip until the statement delimiter token or end of file token is found.</summary>
    private IBoundExpressionNode ParseExpression()
    {
        while (true)
        {
            var tokenCurrent = _tokenWalker.Consume();

            if (tokenCurrent.SyntaxKind == SyntaxKind.EndOfFileToken ||
                tokenCurrent.SyntaxKind == SyntaxKind.StatementDelimiterToken)
            {
                break;
            }
        }

        // #TODO: Correctly implement this method Returning a nonsensical token for now.
        return new BoundLiteralExpressionNode(
            new EndOfFileToken(
                new TextEditorTextSpan(
                    0,
                    0,
                    (byte)GenericDecorationKind.None,
                    new(string.Empty),
                    string.Empty)),
            typeof(void));
    }

    private void ParseOpenBraceToken(
        OpenBraceToken inToken)
    {
        var closureCompilationUnitBuilder = _currentCompilationUnitBuilder;
        Type? scopeReturnType = null;

        if (_nodeRecent is not null &&
            _nodeRecent.SyntaxKind == SyntaxKind.BoundFunctionDefinitionNode)
        {
            var boundFunctionDefinitionNode = (BoundFunctionDefinitionNode)_nodeRecent;

            scopeReturnType = boundFunctionDefinitionNode.ReturnBoundClassReferenceNode.Type;

            _finalizeCompilationUnitAction = compilationUnit =>
            {
                boundFunctionDefinitionNode = boundFunctionDefinitionNode with
                {
                    FunctionBodyCompilationUnit = compilationUnit
                };

                closureCompilationUnitBuilder.Children
                    .Add(boundFunctionDefinitionNode);
            };
        }
        else
        {
            _finalizeCompilationUnitAction = compilationUnit =>
            {
                closureCompilationUnitBuilder.Children
                    .Add(compilationUnit);
            };
        }

        _binder.RegisterBoundScope(
            scopeReturnType,
            inToken.TextSpan);

        _currentCompilationUnitBuilder = new(_currentCompilationUnitBuilder);
    }

    private void ParseCloseBraceToken(
        CloseBraceToken inToken)
    {
        _binder.DisposeBoundScope(inToken.TextSpan);

        if (_currentCompilationUnitBuilder.Parent is not null)
        {
            _finalizeCompilationUnitAction?.Invoke(
                _currentCompilationUnitBuilder.Build());

            _currentCompilationUnitBuilder = _currentCompilationUnitBuilder.Parent;
        }
    }

    private void ParseStatementDelimiterToken()
    {

    }
}
