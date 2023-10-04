using Luthetus.CompilerServices.Lang.DotNetSolution.Code;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Lexer;

public class GlobalSectionExtensibilityGlobalsDoesLex
{
    [Fact]
    public void START_TOKEN_DOES_LEX()
    {
        var sourceText = $"{LexSolutionFacts.GlobalSection.START_TOKEN}({LexSolutionFacts.GlobalSectionExtensibilityGlobals.START_TOKEN})";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            sourceText);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(2, lexer.SyntaxTokens.Length);

        var startToken = (KeywordToken)lexer.SyntaxTokens[0];
        var startParameterToken = (KeywordToken)lexer.SyntaxTokens[1];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());
        Assert.Equal(LexSolutionFacts.GlobalSectionExtensibilityGlobals.START_TOKEN, startParameterToken.TextSpan.GetText());
    }

    [Fact]
    public void FULL_DOES_LEX()
    {
        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataGlobalSectionExtensibilityGlobals.FULL);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(6, lexer.SyntaxTokens.Length);

        var startToken = (KeywordToken)lexer.SyntaxTokens[0];
        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());

        var startParameterToken = (KeywordToken)lexer.SyntaxTokens[1];
        Assert.Equal(LexSolutionFacts.GlobalSectionExtensibilityGlobals.START_TOKEN, startParameterToken.TextSpan.GetText());

        var startOrderToken = (KeywordToken)lexer.SyntaxTokens[2];
        Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.START_TOKEN_ORDER, startOrderToken.TextSpan.GetText());

        var firstPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[3];
        Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.FIRST_PROPERTY_NAME, firstPropertyNameToken.TextSpan.GetText());

        var firstPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[4];
        Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.FIRST_PROPERTY_VALUE, firstPropertyValueToken.TextSpan.GetText());

        var endToken = (KeywordToken)lexer.SyntaxTokens[5];
        Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endToken.TextSpan.GetText());
    }
}