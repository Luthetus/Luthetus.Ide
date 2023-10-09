using Luthetus.CompilerServices.Lang.DotNetSolution.Code;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Lexer;

public class HeaderDoesLex
{
    [Fact]
    public void FORMAT_VERSION_NO_ENDING_NEWLINE_DOES_LEX()
    {
        var formatVersionValue = "12.00";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataHeader.FORMAT_VERSION_NO_ENDING_NEWLINE);

        lexer.Lex();

        Assert.Equal(3, lexer.SyntaxTokens.Length);

        var i = 0;

        var formatVersionAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        var formatVersionAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.Header.FORMAT_VERSION_START_TOKEN, formatVersionAssociatedNameToken.TextSpan.GetText());
        Assert.Equal(formatVersionValue, formatVersionAssociatedValueToken.TextSpan.GetText());
        Assert.NotNull(endOfFileToken);
    }

    [Fact]
    public void HASHTAG_VISUAL_STUDIO_VERSION_DOES_LEX()
    {
        var hashtagVersionValue = "17";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataHeader.HASHTAG_VISUAL_STUDIO_VERSION);

        lexer.Lex();

        Assert.Equal(3, lexer.SyntaxTokens.Length);

        var i = 0;

        var hashtagVersionAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        var hashtagVersionAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.Header.HASHTAG_VISUAL_STUDIO_VERSION_START_TOKEN, hashtagVersionAssociatedNameToken.TextSpan.GetText());
        Assert.Equal(hashtagVersionValue, hashtagVersionAssociatedValueToken.TextSpan.GetText());
        Assert.NotNull(endOfFileToken);
    }

    [Fact]
    public void EXACT_VISUAL_STUDIO_VERSION_DOES_LEX()
    {
        var exactVersionValue = "17.7.34018.315";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataHeader.EXACT_VISUAL_STUDIO_VERSION);

        lexer.Lex();

        Assert.Equal(3, lexer.SyntaxTokens.Length);

        var i = 0;

        var exactVersionAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        var exactVersionAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.Header.EXACT_VISUAL_STUDIO_VERSION_START_TOKEN, exactVersionAssociatedNameToken.TextSpan.GetText());
        Assert.Equal(exactVersionValue, exactVersionAssociatedValueToken.TextSpan.GetText());
        Assert.NotNull(endOfFileToken);
    }

    [Fact]
    public void MINIMUM_VISUAL_STUDIO_VERSION_DOES_LEX()
    {
        var minimumVersionValue = "10.0.40219.1";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataHeader.MINIMUM_VISUAL_STUDIO_VERSION);

        lexer.Lex();

        Assert.Equal(3, lexer.SyntaxTokens.Length);

        var i = 0;

        var minimumVersionAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        var minimumVersionAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.Header.MINIMUM_VISUAL_STUDIO_VERSION_START_TOKEN, minimumVersionAssociatedNameToken.TextSpan.GetText());
        Assert.Equal(minimumVersionValue, minimumVersionAssociatedValueToken.TextSpan.GetText());
        Assert.NotNull(endOfFileToken);
    }

    [Fact]
    public void FULL_DOES_LEX()
    {
        var formatVersionValue = "12.00";
        var hashtagVersionValue = "17";
        var exactVersionValue = "17.7.34018.315";
        var minimumVersionValue = "10.0.40219.1";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataHeader.FULL);

        lexer.Lex();

        var i = 0;

        var formatVersionAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        var formatVersionAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(LexSolutionFacts.Header.FORMAT_VERSION_START_TOKEN, formatVersionAssociatedNameToken.TextSpan.GetText());
        Assert.Equal(formatVersionValue, formatVersionAssociatedValueToken.TextSpan.GetText());

        var hashtagVersionAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        var hashtagVersionAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(LexSolutionFacts.Header.HASHTAG_VISUAL_STUDIO_VERSION_START_TOKEN, hashtagVersionAssociatedNameToken.TextSpan.GetText());
        Assert.Equal(hashtagVersionValue, hashtagVersionAssociatedValueToken.TextSpan.GetText());

        var exactVersionAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        var exactVersionAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(LexSolutionFacts.Header.EXACT_VISUAL_STUDIO_VERSION_START_TOKEN, exactVersionAssociatedNameToken.TextSpan.GetText());
        Assert.Equal(exactVersionValue, exactVersionAssociatedValueToken.TextSpan.GetText());

        var minimumVersionAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        var minimumVersionAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(LexSolutionFacts.Header.MINIMUM_VISUAL_STUDIO_VERSION_START_TOKEN, minimumVersionAssociatedNameToken.TextSpan.GetText());
        Assert.Equal(minimumVersionValue, minimumVersionAssociatedValueToken.TextSpan.GetText());

        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[i++];
        Assert.NotNull(endOfFileToken);
    }
}