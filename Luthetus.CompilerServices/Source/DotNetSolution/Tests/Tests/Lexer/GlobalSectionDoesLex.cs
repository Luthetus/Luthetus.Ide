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

        var i = 0;

        var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());
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

        var i = 0;

        var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
        var startAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());
        Assert.Equal(TestDataGlobalSection.START_TOKEN_PARAMETER, startAssociatedValueToken.TextSpan.GetText());
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

        var i = 0;

        var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
        var startParameterAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        var startOrderAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());
        Assert.Equal(TestDataGlobalSection.START_TOKEN_PARAMETER, startParameterAssociatedValueToken.TextSpan.GetText());
        Assert.Equal(TestDataGlobalSection.START_TOKEN_ORDER, startOrderAssociatedValueToken.TextSpan.GetText());
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

        var i = 0;

        var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
        var startParameterAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        var startOrderAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        var endCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());
        Assert.Equal(TestDataGlobalSection.START_TOKEN_PARAMETER, startParameterAssociatedValueToken.TextSpan.GetText());
        Assert.Equal(TestDataGlobalSection.START_TOKEN_ORDER, startOrderAssociatedValueToken.TextSpan.GetText());
        Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endCloseAssociatedGroupToken.TextSpan.GetText());
    }
}