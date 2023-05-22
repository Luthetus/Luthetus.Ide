using BlazorStudio.ClassLib.CodeAnalysis.C.EvaluatorCase;
using BlazorStudio.ClassLib.CodeAnalysis.C.Syntax;

namespace Luthetus.Ide.Tests.Basics.SemanticParsing.C;

public class EvaluatorTests
{
    [Fact]
    public void SHOULD_EVALUATE_NUMERIC_LITERAL_EXPRESSION()
    {
        var x = 3;

        string sourceText = $"{x}".ReplaceLineEndings("\n");

        var lexer = new LexSession(sourceText);
        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var evaluator = new Evaluator(
            compilationUnit,
            sourceText);

        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x, evaluatorResult.Result);
    }

    [Fact]
    public void SHOULD_EVALUATE_STRING_LITERAL_EXPRESSION()
    {
        var x = "123abc";
        string sourceText = $"\"{x}\"".ReplaceLineEndings("\n");

        var lexer = new LexSession(sourceText);
        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var evaluator = new Evaluator(
            compilationUnit,
            sourceText);

        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(string), evaluatorResult.Type);
        Assert.Equal(x, evaluatorResult.Result);
    }

    [Fact]
    public void SHOULD_EVALUATE_NUMERIC_BINARY_EXPRESSION()
    {
        var x = 3;
        var y = 3;

        string sourceText = $"{x} + {y}".ReplaceLineEndings("\n");

        var lexer = new LexSession(sourceText);
        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var evaluator = new Evaluator(
            compilationUnit,
            sourceText);

        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(int), evaluatorResult.Type);
        Assert.Equal(x + y, evaluatorResult.Result);
    }

    [Fact]
    public void SHOULD_EVALUATE_STRING_BINARY_EXPRESSION()
    {
        var x = "123";
        var y = "abc";

        string sourceText = $"\"{x}\" + \"{y}\"".ReplaceLineEndings("\n");

        var lexer = new LexSession(sourceText);
        lexer.Lex();

        var parser = new ParserSession(
            lexer.SyntaxTokens,
            sourceText,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        var evaluator = new Evaluator(
            compilationUnit,
            sourceText);

        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(string), evaluatorResult.Type);
        Assert.Equal(x + y, evaluatorResult.Result);
    }
}