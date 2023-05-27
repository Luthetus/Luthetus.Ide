
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp;

public class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_NUMERIC_LITERAL_EXPRESSION()
    {
        string sourceText = "3".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var parser = new Parser(
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

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var parser = new Parser(
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

        var lexer = new Lexer(sourceText);
        lexer.Lex();

        var parser = new Parser(
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
    public void SHOULD_NOT_PARSE_COMMENT_SINGLE_LINE_STATEMENT()
    {
        string sourceText = @"// C:\Users\hunte\Repos\Aaa\"
            .ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
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

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
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

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
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

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
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

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
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

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
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

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
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

    /// <summary>GOAL: Add "HelloWorld" key to NamespaceDictionary with a single BoundNamespaceEntryNode child which has a CompilationUnit without any children.</summary>
    [Fact]
    public void SHOULD_PARSE_NAMESPACE_DEFINITION_EMPTY()
    {
        string sourceText = @"namespace HelloWorld {}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var boundNamespaceStatementNode =
            (BoundNamespaceStatementNode)compilationUnit.Children.Single();

        var boundNamespaceEntryNode =
            (BoundNamespaceEntryNode)boundNamespaceStatementNode.Children.Single();

        var namespaceEntryCompilationUnit =
            (CompilationUnit)boundNamespaceEntryNode.Children.Single();

        Assert.Empty(namespaceEntryCompilationUnit.Children);

        // Assert SyntaxKinds are correct
        {
            Assert.Equal(
                SyntaxKind.BoundNamespaceStatementNode,
                boundNamespaceStatementNode.SyntaxKind);

            Assert.Equal(
                SyntaxKind.BoundNamespaceEntryNode,
                boundNamespaceEntryNode.SyntaxKind);

            Assert.Equal(
                SyntaxKind.CompilationUnit,
                namespaceEntryCompilationUnit.SyntaxKind);
        }
    }

    [Fact]
    public void SHOULD_PARSE_TWO_NAMESPACE_DECLARATIONS_WITH_THE_SAME_IDENTIFIER_INTO_A_SINGLE_SCOPE()
    {
        /*
         * GOAL: Add "PersonCase" key to NamespaceDictionary with
         *       two CompilationUnit children.
         *           -PersonModel.cs
         *           -PersonDisplay.razor.cs
         *           
         *       Afterwards evaluate the Namespace as a BoundScope
         *       which would contain the two classes:
         *          -PersonModel
         *          -PersonDisplay
         */

        string personModelFile = @"namespace PersonCase
{
    public class PersonModel
    {
    }
}".ReplaceLineEndings("\n");

        string personDisplayFile = @"namespace PersonCase
{
    public partial class PersonDisplay : ComponentBase
    {
    }
}".ReplaceLineEndings("\n");

        CompilationUnit personModelFileCompilationUnit;
        CompilationUnit personDisplayFileCompilationUnit;

        // Parse personModelFile
        {
            var lexer = new Lexer(personModelFile);

            lexer.Lex();

            var parser = new Parser(
                lexer.SyntaxTokens,
                personModelFile,
                lexer.Diagnostics);

            personModelFileCompilationUnit = parser.Parse();
        }

        // Parse personModelFile
        {
            var lexer = new Lexer(personDisplayFile);

            lexer.Lex();

            var parser = new Parser(
                lexer.SyntaxTokens,
                personDisplayFile,
                lexer.Diagnostics);

            personDisplayFileCompilationUnit = parser.Parse();
        }

        throw new NotImplementedException();
    }
    
    [Fact]
    public void SHOULD_PARSE_USING_STATEMENT()
    {
        /*
         * GOAL: Add "PersonCase" key to NamespaceDictionary with
         *       two CompilationUnit children.
         *           -PersonModel.cs
         *           -PersonDisplay.razor.cs
         *           
         *       Afterwards evaluate the Namespace as a BoundScope
         *       which would contain the two classes:
         *          -PersonModel
         *          -PersonDisplay
         *      
         *      Afterwards add "Pages" key to NamespaceDictionary
         *      with one CompilationUnit child.
         *          -PersonPage.razor
         */

        string sourceText = @"namespace HelloWorld {}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        throw new NotImplementedException();
    }
}