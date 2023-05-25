using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.ParserCase;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.C;

public class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_NUMERIC_LITERAL_EXPRESSION()
    {
        string sourceText = "3".ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);
        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(typeof(int), boundLiteralExpressionNode.ResultType);
    }

    [Fact]
    public void SHOULD_PARSE_STRING_LITERAL_EXPRESSION()
    {
        string sourceText = "\"123abc\"".ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);
        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundLiteralExpressionNode = (BoundLiteralExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(typeof(string), boundLiteralExpressionNode.ResultType);
    }

    [Fact]
    public void SHOULD_PARSE_NUMERIC_BINARY_EXPRESSION()
    {
        string sourceText = "3 + 3".ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);
        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundBinaryExpressionNode = (BoundBinaryExpressionNode)compilationUnit
            .Children[0];

        Assert.Equal(
            typeof(int),
            boundBinaryExpressionNode.LeftBoundExpressionNode.ResultType);

        Assert.Equal(
            typeof(int),
            boundBinaryExpressionNode.BoundBinaryOperatorNode.ResultType);

        Assert.Equal(
            typeof(int),
            boundBinaryExpressionNode.RightBoundExpressionNode.ResultType);
    }

    [Fact]
    public void SHOULD_PARSE_LIBRARY_REFERENCE()
    {
        string sourceText = "#include <stdlib.h>"
            .ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);

        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var libraryReferenceNode = compilationUnit.Children.Single();

        Assert.Equal(
            SyntaxKind.PreprocessorLibraryReferenceStatement,
            libraryReferenceNode.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_TWO_LIBRARY_REFERENCES()
    {
        string sourceText = @"#include <stdlib.h>
#include <stdio.h>"
            .ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);

        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Equal(2, compilationUnit.Children.Length);

        var firstLibraryReferenceNode = compilationUnit.Children.First();

        Assert.Equal(
            SyntaxKind.PreprocessorLibraryReferenceStatement,
            firstLibraryReferenceNode.SyntaxKind);

        var secondLibraryReferenceNode = compilationUnit.Children.Last();

        Assert.Equal(
            SyntaxKind.PreprocessorLibraryReferenceStatement,
            secondLibraryReferenceNode.SyntaxKind);
    }

    [Fact]
    public void SHOULD_NOT_PARSE_COMMENT_SINGLE_LINE_STATEMENT()
    {
        string sourceText = @"// C:\Users\hunte\Repos\Aaa\"
            .ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);

        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Empty(compilationUnit.Children);
    }

    [Fact]
    public void SHOULD_PARSE_VARIABLE_DECLARATION_STATEMENT()
    {
        string sourceText = @"int x;"
            .ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);

        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundVariableDeclarationStatementNode =
            (BoundVariableDeclarationStatementNode)compilationUnit.Children
                .Single();

        Assert.Equal(
            SyntaxKind.BoundVariableDeclarationStatementNode,
            boundVariableDeclarationStatementNode.SyntaxKind);

        Assert.Equal(
            2,
            boundVariableDeclarationStatementNode.Children.Length);

        var boundTypeNode = (BoundTypeNode)boundVariableDeclarationStatementNode
            .Children[0];

        Assert.Equal(
            SyntaxKind.BoundTypeNode,
            boundTypeNode.SyntaxKind);

        Assert.Equal(
            typeof(int),
            boundTypeNode.Type);

        var identifierToken = boundVariableDeclarationStatementNode.Children[1];

        Assert.Equal(
            SyntaxKind.IdentifierToken,
            identifierToken.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_VARIABLE_DECLARATION_STATEMENT_THEN_VARIABLE_ASSIGNMENT_STATEMENT()
    {
        string sourceText = @"int x;
x = 42;"
            .ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);

        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Equal(2, compilationUnit.Children.Length);

        var boundVariableDeclarationStatementNode =
            (BoundVariableDeclarationStatementNode)compilationUnit.Children[0];

        Assert.Equal(
            SyntaxKind.BoundVariableDeclarationStatementNode,
            boundVariableDeclarationStatementNode.SyntaxKind);

        var boundVariableAssignmentStatementNode =
            (BoundVariableAssignmentStatementNode)compilationUnit.Children[1];

        Assert.Equal(
            SyntaxKind.BoundVariableAssignmentStatementNode,
            boundVariableAssignmentStatementNode.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_COMPOUND_VARIABLE_DECLARATION_AND_ASSIGNMENT_STATEMENT()
    {
        string sourceText = @"int x = 42;"
            .ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);

        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Equal(2, compilationUnit.Children.Length);

        var boundVariableDeclarationStatementNode =
            (BoundVariableDeclarationStatementNode)compilationUnit.Children[0];

        Assert.Equal(
            SyntaxKind.BoundVariableDeclarationStatementNode,
            boundVariableDeclarationStatementNode.SyntaxKind);

        var boundVariableAssignmentStatementNode =
            (BoundVariableAssignmentStatementNode)compilationUnit.Children[1];

        Assert.Equal(
            SyntaxKind.BoundVariableAssignmentStatementNode,
            boundVariableAssignmentStatementNode.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT()
    {
        string sourceText = @"void WriteHelloWorldToConsole()
{
}"
            .ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);

        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Single(compilationUnit.Children);

        var boundFunctionDeclarationNode =
            (BoundFunctionDeclarationNode)compilationUnit.Children[0];

        Assert.Equal(
            SyntaxKind.BoundFunctionDeclarationNode,
            boundFunctionDeclarationNode.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT()
    {
        string sourceText = @"void WriteHelloWorldToConsole()
{
}

WriteHelloWorldToConsole();"
            .ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);

        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Equal(2, compilationUnit.Children.Length);

        var boundFunctionDeclarationNode =
            (BoundFunctionDeclarationNode)compilationUnit.Children[0];

        Assert.Equal(
            SyntaxKind.BoundFunctionDeclarationNode,
            boundFunctionDeclarationNode.SyntaxKind);

        var boundFunctionInvocationNode =
            (BoundFunctionInvocationNode)compilationUnit.Children[1];

        Assert.Equal(
            SyntaxKind.BoundFunctionInvocationNode,
            boundFunctionInvocationNode.SyntaxKind);
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_WITH_DIAGNOSTIC_FOR_UNDEFINED_FUNCTION()
    {
        string sourceText = @"printf();"
            .ReplaceLineEndings("\n");

        var lexer = new LexerSession(sourceText);

        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        // BoundFunctionInvocationNode Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundFunctionInvocationNode =
                (BoundFunctionInvocationNode)compilationUnit.Children.Single();

            Assert.Equal(
                SyntaxKind.BoundFunctionInvocationNode,
                boundFunctionInvocationNode.SyntaxKind);
        }

        // Diagnostic Assertions
        {
            Assert.Single(compilationUnit.Diagnostics);

            var errorDiagnostic = compilationUnit.Diagnostics
                .Single();

            Assert.Equal(
                TextEditorDiagnosticLevel.Error,
                errorDiagnostic.DiagnosticLevel);
        }
    }
}