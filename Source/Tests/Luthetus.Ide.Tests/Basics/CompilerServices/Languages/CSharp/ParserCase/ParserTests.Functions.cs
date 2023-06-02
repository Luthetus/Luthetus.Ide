using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
    [Fact]
    public void SHOULD_PARSE_FUNCTION_DEFINITION_STATEMENT()
    {
        string sourceText = @"void WriteHelloWorldToConsole()
{
}"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
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

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
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

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
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
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_CLASS_INSTANCE()
    {
        var sourceText = @"TODO"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_A_CLASS_INSTANCE_WHICH_IS_NESTED_A_MEMBER_ACCESS_EXPRESSION()
    {
        var sourceText = @"namespace PersonCase;

public class PersonModel
{
    public BodyModel BodyModel { get; set; }
}

public class BodyModel
{
    public void Walk()
    {
    }
}

public class World
{
    private PersonModel _person = new PersonModel();

    public void Tick()
    {
        _person.BodyModel.Walk();
    }
}"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_STATIC_CLASS()
    {
        var sourceText = @"
using System;

Console.WriteLine(""Hello World!"");"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }

    [Fact]
    public void SHOULD_PARSE_METHOD_INVOCATION_ON_STATIC_CLASS_WITH_EXPLICIT_NAMESPACE_QUALIFICATION()
    {
        var sourceText = @"System.Console.WriteLine(""Hello World!"");"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var modelParser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = modelParser.Parse();

        throw new NotImplementedException();
    }
}
