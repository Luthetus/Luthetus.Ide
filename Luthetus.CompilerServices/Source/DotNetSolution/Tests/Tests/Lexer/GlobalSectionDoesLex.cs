using Luthetus.CompilerServices.Lang.DotNetSolution.Code;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Lexer;

public class GlobalSectionDoesLex
{
    [Fact]
    public void START_TOKEN_DOES_LEX()
    {
        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            LexSolutionFacts.GlobalSection.START_TOKEN);

        lexer.LexGlobalSection(() => false);

        Assert.Single(lexer.SyntaxTokens);

        var startToken = (KeywordToken)lexer.SyntaxTokens[0];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());
    }

    [Fact]
    public void START_TOKEN_PARAMETER_DOES_LEX()
    {
        var sourceText = $"{LexSolutionFacts.GlobalSection.START_TOKEN}({TestDataGlobalSection.START_TOKEN_PARAMETER})";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            sourceText);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(2, lexer.SyntaxTokens.Length);

        var startToken = (KeywordToken)lexer.SyntaxTokens[0];
        var startParameterToken = (KeywordToken)lexer.SyntaxTokens[1];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());
        Assert.Equal(TestDataGlobalSection.START_TOKEN_PARAMETER, startParameterToken.TextSpan.GetText());
    }

    [Fact]
    public void START_TOKEN_ORDER_DOES_LEX()
    {
        var sourceText = $"{LexSolutionFacts.GlobalSection.START_TOKEN}({TestDataGlobalSection.START_TOKEN_PARAMETER}) = {TestDataGlobalSection.START_TOKEN_ORDER}";
        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            sourceText);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(3, lexer.SyntaxTokens.Length);

        var startToken = (KeywordToken)lexer.SyntaxTokens[0];
        var startParameterToken = (KeywordToken)lexer.SyntaxTokens[1];
        var startOrderToken = (KeywordToken)lexer.SyntaxTokens[2];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());
        Assert.Equal(TestDataGlobalSection.START_TOKEN_PARAMETER, startParameterToken.TextSpan.GetText());
        Assert.Equal(TestDataGlobalSection.START_TOKEN_ORDER, startOrderToken.TextSpan.GetText());
    }

    [Fact]
    public void FULL_DOES_LEX()
    {
        var sourceText = $"{LexSolutionFacts.GlobalSection.START_TOKEN}({TestDataGlobalSection.START_TOKEN_PARAMETER}) = {TestDataGlobalSection.START_TOKEN_ORDER}\n{LexSolutionFacts.GlobalSection.END_TOKEN}";
        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            sourceText);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(4, lexer.SyntaxTokens.Length);

        var startToken = (KeywordToken)lexer.SyntaxTokens[0];
        var startParameterToken = (KeywordToken)lexer.SyntaxTokens[1];
        var startOrderToken = (KeywordToken)lexer.SyntaxTokens[2];
        var endToken = (KeywordToken)lexer.SyntaxTokens[3];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());
        Assert.Equal(TestDataGlobalSection.START_TOKEN_PARAMETER, startParameterToken.TextSpan.GetText());
        Assert.Equal(TestDataGlobalSection.START_TOKEN_ORDER, startOrderToken.TextSpan.GetText());
        Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endToken.TextSpan.GetText());
    }
}