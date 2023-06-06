using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT_NO_ARGUMENTS()
    {
        string sourceText = @"void WriteHelloWorldToConsole(){}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundFunctionDeclarationNode =
                (BoundFunctionDeclarationNode)compilationUnit.Children[0];

            Assert.Equal(
                SyntaxKind.BoundFunctionDeclarationNode,
                boundFunctionDeclarationNode.SyntaxKind);
        }
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT_WITH_ONE_ARGUMENT()
    {
        string sourceText = @"void WriteHelloWorldToConsole(int times) {}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundFunctionDeclarationNode =
                (BoundFunctionDeclarationNode)compilationUnit.Children[0];

            Assert.Equal(
                SyntaxKind.BoundFunctionDeclarationNode,
                boundFunctionDeclarationNode.SyntaxKind);
        }
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT_WITH_TWO_ARGUMENT()
    {
        string sourceText = @"void WriteHelloWorldToConsole(int times, bool usePurpleText){}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundFunctionDeclarationNode =
                (BoundFunctionDeclarationNode)compilationUnit.Children[0];

            Assert.Equal(
                SyntaxKind.BoundFunctionDeclarationNode,
                boundFunctionDeclarationNode.SyntaxKind);
        }
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT_WITH_THREE_ARGUMENT()
    {
        string sourceText = @"void WriteHelloWorldToConsole(int times, bool usePurpleText){}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            Assert.Single(compilationUnit.Children);

            var boundFunctionDeclarationNode =
                (BoundFunctionDeclarationNode)compilationUnit.Children[0];

            Assert.Equal(
                SyntaxKind.BoundFunctionDeclarationNode,
                boundFunctionDeclarationNode.SyntaxKind);
        }
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT()
    {
        string sourceText = @"void WriteHelloWorldToConsole(){} WriteHelloWorldToConsole();".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
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
    }

    [Fact]
    public void SHOULD_PARSE_FUNCTION_INVOCATION_STATEMENT_WITH_DIAGNOSTIC_FOR_UNDEFINED_FUNCTION()
    {
        string sourceText = @"printf();".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            // BoundFunctionInvocationNode
            Assert.Single(compilationUnit.Children);

            var boundFunctionInvocationNode =
                (BoundFunctionInvocationNode)compilationUnit.Children.Single();

            Assert.Equal(
                SyntaxKind.BoundFunctionInvocationNode,
                boundFunctionInvocationNode.SyntaxKind);

            // Diagnostic
            Assert.Single(compilationUnit.Diagnostics);

            var errorDiagnostic = compilationUnit.Diagnostics
                .Single();

            Assert.Equal(
                TextEditorDiagnosticLevel.Error,
                errorDiagnostic.DiagnosticLevel);
        }
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_CLASS_INSTANCE()
    {
        var sourceText = @"TODO".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_STATIC_CLASS_WITH_USING_STATEMENT()
    {
        var sourceText = @"using System; Console.WriteLine(""Hello World!"");".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_STATIC_CLASS_WITH_EXPLICIT_NAMESPACE_QUALIFICATION()
    {
        var sourceText = @"System.Console.WriteLine(""Hello World!"");".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_DEFINITION_WITH_GENERIC_ARGUMENT()
    {
        var sourceText = @"public T Clone<T>(T item) { return item; }".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_DEFINITION_WITH_GENERIC_ARGUMENT_CLAUSE()
    {
        var sourceText = @"public T Clone<T>(T item) where T : class { return item; }".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_WITH_GENERIC_ARGUMENT()
    {
        var sourceText = @"Clone<int>(3){}".ReplaceLineEndings("\n");
        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new Parser(lexer.SyntaxTokens, lexer.Diagnostics);
        var compilationUnit = parser.Parse();

        // Assertions
        {
            throw new NotImplementedException();
        }
    }
}
