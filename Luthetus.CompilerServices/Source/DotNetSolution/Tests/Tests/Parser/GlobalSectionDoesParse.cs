using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Parser;

public class GlobalSectionDoesParse
{
    [Fact]
    public void FULL_DOES_PARSE()
    {
        var sourceText = $"{LexSolutionFacts.GlobalSection.START_TOKEN}({TestDataGlobalSection.START_TOKEN_PARAMETER}) = {TestDataGlobalSection.START_TOKEN_ORDER}\n{LexSolutionFacts.GlobalSection.END_TOKEN}";
        var lexer = new DotNetSolutionLexer(
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