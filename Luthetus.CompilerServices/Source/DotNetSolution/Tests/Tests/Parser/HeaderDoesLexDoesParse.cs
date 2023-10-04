using Luthetus.CompilerServices.Lang.DotNetSolution.Code;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Parser;

public class HeaderDoesLexDoesParse
{
    [Fact]
    public void FULL_DOES_PARSE()
    {
        var formatVersionValue = "12.00";
        var hashtagVersionValue = "17";
        var exactVersionValue = "17.7.34018.315";
        var minimumVersionValue = "10.0.40219.1";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataHeader.FULL);

        lexer.Lex();

        var formatVersionKeywordToken = (KeywordToken)lexer.SyntaxTokens[0];
        var formatVersionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[1];
        Assert.Equal(LexSolutionFacts.Header.FORMAT_VERSION_START_TOKEN, formatVersionKeywordToken.TextSpan.GetText());
        Assert.Equal(formatVersionValue, formatVersionIdentifierToken.TextSpan.GetText());

        var hashtagVersionKeywordToken = (KeywordToken)lexer.SyntaxTokens[2];
        var hashtagVersionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[3];
        Assert.Equal(LexSolutionFacts.Header.HASHTAG_VISUAL_STUDIO_VERSION_START_TOKEN, hashtagVersionKeywordToken.TextSpan.GetText());
        Assert.Equal(hashtagVersionValue, hashtagVersionIdentifierToken.TextSpan.GetText());

        var exactVersionKeywordToken = (KeywordToken)lexer.SyntaxTokens[4];
        var exactVersionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[5];
        Assert.Equal(LexSolutionFacts.Header.EXACT_VISUAL_STUDIO_VERSION_START_TOKEN, exactVersionKeywordToken.TextSpan.GetText());
        Assert.Equal(exactVersionValue, exactVersionIdentifierToken.TextSpan.GetText());

        var minimumVersionKeywordToken = (KeywordToken)lexer.SyntaxTokens[6];
        var minimumVersionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[7];
        Assert.Equal(LexSolutionFacts.Header.MINIMUM_VISUAL_STUDIO_VERSION_START_TOKEN, minimumVersionKeywordToken.TextSpan.GetText());
        Assert.Equal(minimumVersionValue, minimumVersionIdentifierToken.TextSpan.GetText());

        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[8];
        Assert.NotNull(endOfFileToken);
    }
}