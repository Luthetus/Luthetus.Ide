using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Parser;

public class GlobalSectionProjectConfigurationPlatformsDoesParse
{
    [Fact]
    public void FULL_DOES_PARSE()
    {
        var lexer = new DotNetSolutionLexer(
            new(string.Empty),
            TestDataGlobalSectionProjectConfigurationPlatforms.FULL);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(12, lexer.SyntaxTokens.Length);

        var startToken = (KeywordToken)lexer.SyntaxTokens[0];
        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());

        var startParameterToken = (KeywordToken)lexer.SyntaxTokens[1];
        Assert.Equal(LexSolutionFacts.GlobalSectionProjectConfigurationPlatforms.START_TOKEN, startParameterToken.TextSpan.GetText());

        var startOrderToken = (KeywordToken)lexer.SyntaxTokens[2];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.START_TOKEN_ORDER, startOrderToken.TextSpan.GetText());

        var firstPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[3];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FIRST_PROPERTY_NAME, firstPropertyNameToken.TextSpan.GetText());

        var firstPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[4];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FIRST_PROPERTY_VALUE, firstPropertyValueToken.TextSpan.GetText());

        var secondPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[5];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.SECOND_PROPERTY_NAME, secondPropertyNameToken.TextSpan.GetText());

        var secondPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[6];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.SECOND_PROPERTY_VALUE, secondPropertyValueToken.TextSpan.GetText());

        var thirdPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[7];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.THIRD_PROPERTY_NAME, thirdPropertyNameToken.TextSpan.GetText());

        var thirdPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[8];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.THIRD_PROPERTY_VALUE, thirdPropertyValueToken.TextSpan.GetText());

        var fourthPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[9];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FOURTH_PROPERTY_NAME, fourthPropertyNameToken.TextSpan.GetText());

        var fourthPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[10];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FOURTH_PROPERTY_VALUE, fourthPropertyValueToken.TextSpan.GetText());

        var endToken = (KeywordToken)lexer.SyntaxTokens[11];
        Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endToken.TextSpan.GetText());
    }
}