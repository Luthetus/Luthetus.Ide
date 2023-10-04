using Luthetus.CompilerServices.Lang.DotNetSolution.Code;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Parser;

public class GlobalSectionSolutionPropertiesDoesParse
{
    
    [Fact]
    public void FULL_DOES_PARSE()
    {
        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataGlobalSectionSolutionProperties.FULL);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(6, lexer.SyntaxTokens.Length);

        var startToken = (KeywordToken)lexer.SyntaxTokens[0];
        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());

        var startParameterToken = (KeywordToken)lexer.SyntaxTokens[1];
        Assert.Equal(LexSolutionFacts.GlobalSectionSolutionProperties.START_TOKEN, startParameterToken.TextSpan.GetText());

        var startOrderToken = (KeywordToken)lexer.SyntaxTokens[2];
        Assert.Equal(TestDataGlobalSectionSolutionProperties.START_TOKEN_ORDER, startOrderToken.TextSpan.GetText());

        var solutionGuidNameToken = (IdentifierToken)lexer.SyntaxTokens[3];
        Assert.Equal(LexSolutionFacts.GlobalSectionSolutionProperties.HIDE_SOLUTION_NODE_NAME, solutionGuidNameToken.TextSpan.GetText());

        var solutionGuidValueToken = (IdentifierToken)lexer.SyntaxTokens[4];
        Assert.Equal(TestDataGlobalSectionSolutionProperties.HIDE_SOLUTION_NODE_VALUE, solutionGuidValueToken.TextSpan.GetText());

        var endToken = (KeywordToken)lexer.SyntaxTokens[5];
        Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endToken.TextSpan.GetText());
    }
}