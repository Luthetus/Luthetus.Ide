using Luthetus.CompilerServices.Lang.DotNetSolution.Code;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Lexer;

public class GlobalSectionProjectConfigurationPlatformsDoesLex
{
    [Fact]
    public void START_TOKEN_DOES_LEX()
    {
        var sourceText = $"{LexSolutionFacts.GlobalSection.START_TOKEN}({LexSolutionFacts.GlobalSectionProjectConfigurationPlatforms.START_TOKEN})";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            sourceText);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(2, lexer.SyntaxTokens.Length);

        var i = 0;

        var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
        var startParameterAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());
        Assert.Equal(LexSolutionFacts.GlobalSectionProjectConfigurationPlatforms.START_TOKEN, startParameterAssociatedValueToken.TextSpan.GetText());
    }

    [Fact]
    public void FULL_DOES_LEX()
    {
        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataGlobalSectionProjectConfigurationPlatforms.FULL);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(12, lexer.SyntaxTokens.Length);

        var i = 0;

        var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());

        var startParameterAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(LexSolutionFacts.GlobalSectionProjectConfigurationPlatforms.START_TOKEN, startParameterAssociatedValueToken.TextSpan.GetText());

        var startOrderAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.START_TOKEN_ORDER, startOrderAssociatedValueToken.TextSpan.GetText());

        var firstPropertyNameAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FIRST_PROPERTY_NAME, firstPropertyNameAssociatedNameToken.TextSpan.GetText());

        var firstPropertyValueAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FIRST_PROPERTY_VALUE, firstPropertyValueAssociatedValueToken.TextSpan.GetText());

        var secondPropertyNameAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.SECOND_PROPERTY_NAME, secondPropertyNameAssociatedNameToken.TextSpan.GetText());

        var secondPropertyValueAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.SECOND_PROPERTY_VALUE, secondPropertyValueAssociatedValueToken.TextSpan.GetText());

        var thirdPropertyNameAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.THIRD_PROPERTY_NAME, thirdPropertyNameAssociatedNameToken.TextSpan.GetText());

        var thirdPropertyValueAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.THIRD_PROPERTY_VALUE, thirdPropertyValueAssociatedValueToken.TextSpan.GetText());

        var fourthPropertyNameAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FOURTH_PROPERTY_NAME, fourthPropertyNameAssociatedNameToken.TextSpan.GetText());

        var fourthPropertyValueAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FOURTH_PROPERTY_VALUE, fourthPropertyValueAssociatedValueToken.TextSpan.GetText());

        var endCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[i++];
        Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endCloseAssociatedGroupToken.TextSpan.GetText());
    }
}