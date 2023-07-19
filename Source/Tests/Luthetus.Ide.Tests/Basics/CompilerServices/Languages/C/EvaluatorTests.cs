using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.EvaluatorCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.C;

public class EvaluatorTests
{
    [Fact]
    public void SHOULD_EVALUATE_NUMERIC_LITERAL_EXPRESSION()
    {
        var x = 3;

        string sourceText = $"{x}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CLexerSession(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new CParserSession(lexer);

        var compilationUnit = parser.Parse();

        var evaluator = new CEvaluator(
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

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CLexerSession(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new CParserSession(lexer);

        var compilationUnit = parser.Parse();

        var evaluator = new CEvaluator(
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

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CLexerSession(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new CParserSession(lexer);

        var compilationUnit = parser.Parse();

        var evaluator = new CEvaluator(
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

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new CLexerSession(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new CParserSession(lexer);

        var compilationUnit = parser.Parse();

        var evaluator = new CEvaluator(
            compilationUnit,
            sourceText);

        var evaluatorResult = evaluator.Evaluate();

        Assert.Equal(typeof(string), evaluatorResult.Type);
        Assert.Equal(x + y, evaluatorResult.Result);
    }
}