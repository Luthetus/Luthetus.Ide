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

    [Fact]
    public void SHOULD_PARSE_CLASS_DECLARATION_EMPTY()
    {
        string classIdentifier = "PersonModel";

        string sourceText = @$"public class {classIdentifier}
{{
}}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var globalScope = parser.Binder.BoundScopes.First();

        var personModel = globalScope.ClassDeclarationMap.Single();

        Assert.Equal(classIdentifier, personModel.Key);

        var boundClassDeclarationNode = 
            (BoundClassDeclarationNode)compilationUnit.Children.Single();

        if (boundClassDeclarationNode.ClassBodyCompilationUnit is null)
            throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

        Assert.Empty(
            boundClassDeclarationNode.ClassBodyCompilationUnit.Children);
    }
    
    [Fact]
    public void SHOULD_PARSE_CLASS_DECLARATION_WHICH_CONTAINS_A_METHOD()
    {
        string classIdentifier = "PersonModel";
        string methodIdentifier = "Walk";

        string sourceText = @$"public class {classIdentifier}
{{
    public void {methodIdentifier}()
    {{
    }}
}}".ReplaceLineEndings("\n");

        var lexer = new Lexer(sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var globalScope = parser.Binder.BoundScopes.First();

        var personModel = globalScope.ClassDeclarationMap.Single();

        Assert.Equal(classIdentifier, personModel.Key);

        var boundClassDeclarationNode = 
            (BoundClassDeclarationNode)compilationUnit.Children.Single();

        if (boundClassDeclarationNode.ClassBodyCompilationUnit is null)
            throw new ApplicationException("ClassBodyCompilationUnit should not be null here.");

        var boundFunctionDeclarationNode = 
            (BoundFunctionDeclarationNode)boundClassDeclarationNode
                .ClassBodyCompilationUnit.Children.Single();

        Assert.Equal(
            SyntaxKind.BoundFunctionDeclarationNode,
            boundFunctionDeclarationNode.SyntaxKind);
    }

    /// <summary>GOAL: Add "HelloWorld" key to NamespaceDictionary with a single CompilationUnit child which has a CompilationUnit without any children.</summary>
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

        var namespaceCompilationUnit =
            (CompilationUnit)boundNamespaceStatementNode.Children.Single();

        Assert.Empty(namespaceCompilationUnit.Children);

        // Assert SyntaxKinds are correct
        {
            Assert.Equal(
                SyntaxKind.BoundNamespaceStatementNode,
                boundNamespaceStatementNode.SyntaxKind);

            Assert.Equal(
                SyntaxKind.CompilationUnit,
                namespaceCompilationUnit.SyntaxKind);
        }
    }

    /// <summary>GOAL: Add "PersonCase" key to NamespaceDictionary with two CompilationUnit children: 'PersonModel.cs', and 'PersonDisplay.razor.cs'.<br/><br/>Afterwards convert the Namespace to a BoundScope which would contain the two classes: 'PersonModel', and 'PersonDisplay'</summary>
    [Fact]
    public void SHOULD_PARSE_TWO_NAMESPACE_DECLARATIONS_WITH_THE_SAME_IDENTIFIER_INTO_A_SINGLE_SCOPE()
    {
        var personModelFile = new TestResource(
            "PersonModel.cs",
            @"namespace PersonCase
{
    public class PersonModel
    {
    }
}".ReplaceLineEndings("\n"));

        var personDisplayFile = new TestResource(
            "PersonDisplay.razor.cs",
            @"namespace PersonCase
{
    public partial class PersonDisplay : ComponentBase
    {
    }
}".ReplaceLineEndings("\n"));

        CompilationUnit personModelFileCompilationUnit;
        CompilationUnit personDisplayFileCompilationUnit;

        // Parse personModelFile
        {
            var lexer = new Lexer(personModelFile.Content);

            lexer.Lex();

            var parser = new Parser(
                lexer.SyntaxTokens,
                personModelFile.Content,
                lexer.Diagnostics,
                "PersonModel.cs");

            personModelFileCompilationUnit = parser.Parse();
        }

        // Parse personModelFile
        {
            var lexer = new Lexer(personDisplayFile.Content);

            lexer.Lex();

            var parser = new Parser(
                lexer.SyntaxTokens,
                personDisplayFile.Content,
                lexer.Diagnostics,
                "PersonDisplay.razor.cs");

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