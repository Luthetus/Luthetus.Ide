using Luthetus.CompilerServices.Lang.DotNetSolution.Code;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Lexer;

public class MainDoesLex
{
    [Fact]
    public void MINIMAL_EXAMPLE_DOES_LEX()
    {
        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataMain.MINIMAL_EXAMPLE);

        lexer.Lex();

        var i = 0;

        // Header
        {
            var formatVersionValue = "12.00";
            var hashtagVersionValue = "17";
            var exactVersionValue = "17.7.34018.315";
            var minimumVersionValue = "10.0.40219.1";

            var formatVersionKeywordToken = (KeywordToken)lexer.SyntaxTokens[i++];
            var formatVersionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Header.FORMAT_VERSION_START_TOKEN, formatVersionKeywordToken.TextSpan.GetText());
            Assert.Equal(formatVersionValue, formatVersionIdentifierToken.TextSpan.GetText());

            var hashtagVersionKeywordToken = (KeywordToken)lexer.SyntaxTokens[i++];
            var hashtagVersionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Header.HASHTAG_VISUAL_STUDIO_VERSION_START_TOKEN, hashtagVersionKeywordToken.TextSpan.GetText());
            Assert.Equal(hashtagVersionValue, hashtagVersionIdentifierToken.TextSpan.GetText());

            var exactVersionKeywordToken = (KeywordToken)lexer.SyntaxTokens[i++];
            var exactVersionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Header.EXACT_VISUAL_STUDIO_VERSION_START_TOKEN, exactVersionKeywordToken.TextSpan.GetText());
            Assert.Equal(exactVersionValue, exactVersionIdentifierToken.TextSpan.GetText());

            var minimumVersionKeywordToken = (KeywordToken)lexer.SyntaxTokens[i++];
            var minimumVersionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Header.MINIMUM_VISUAL_STUDIO_VERSION_START_TOKEN, minimumVersionKeywordToken.TextSpan.GetText());
            Assert.Equal(minimumVersionValue, minimumVersionIdentifierToken.TextSpan.GetText());
        }

        // Project
        {
            var projectTypeGuidValue = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
            var nameNoExtensionValue = "ConsoleApp2";
            var relativePathFromSlnValue = "ConsoleApp2\\ConsoleApp2.csproj";
            var projectIdGuidValue = "623099D9-D9DE-47E8-8CCD-BC301C82F70F";

            var startProjectDefinitionKeywordToken = (KeywordToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN, startProjectDefinitionKeywordToken.TextSpan.GetText());

            var projectTypeGuidIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[i++];
            Assert.Equal(projectTypeGuidValue, projectTypeGuidIdentifierToken.TextSpan.GetText());

            var nameNoExtensionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[i++];
            Assert.Equal(nameNoExtensionValue, nameNoExtensionIdentifierToken.TextSpan.GetText());

            var relativePathFromSlnIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[i++];
            Assert.Equal(relativePathFromSlnValue, relativePathFromSlnIdentifierToken.TextSpan.GetText());

            var projectIdGuidIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[i++];
            Assert.Equal(projectIdGuidValue, projectIdGuidIdentifierToken.TextSpan.GetText());

            var endProjectDefinitionKeywordToken = (KeywordToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Project.PROJECT_DEFINITION_END_TOKEN, endProjectDefinitionKeywordToken.TextSpan.GetText());
        }

        // Global
        {
            var globalStartToken = (KeywordToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Global.START_TOKEN, globalStartToken.TextSpan.GetText());

            // GlobalSection(SolutionConfigurationPlatforms)
            {
                var startToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());

                var startParameterToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionSolutionConfigurationPlatforms.START_TOKEN, startParameterToken.TextSpan.GetText());

                var startOrderToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionConfigurationPlatforms.START_TOKEN_ORDER, startOrderToken.TextSpan.GetText());

                var firstPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionConfigurationPlatforms.FIRST_PROPERTY_NAME, firstPropertyNameToken.TextSpan.GetText());

                var firstPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionConfigurationPlatforms.FIRST_PROPERTY_VALUE, firstPropertyValueToken.TextSpan.GetText());

                var secondPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionConfigurationPlatforms.SECOND_PROPERTY_NAME, secondPropertyNameToken.TextSpan.GetText());

                var secondPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionConfigurationPlatforms.SECOND_PROPERTY_VALUE, secondPropertyValueToken.TextSpan.GetText());

                var endToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endToken.TextSpan.GetText());
            }

            // GlobalSection(ProjectConfigurationPlatforms)
            {
                var startToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());

                var startParameterToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionProjectConfigurationPlatforms.START_TOKEN, startParameterToken.TextSpan.GetText());

                var startOrderToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.START_TOKEN_ORDER, startOrderToken.TextSpan.GetText());

                var firstPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FIRST_PROPERTY_NAME, firstPropertyNameToken.TextSpan.GetText());

                var firstPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FIRST_PROPERTY_VALUE, firstPropertyValueToken.TextSpan.GetText());

                var secondPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.SECOND_PROPERTY_NAME, secondPropertyNameToken.TextSpan.GetText());

                var secondPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.SECOND_PROPERTY_VALUE, secondPropertyValueToken.TextSpan.GetText());

                var thirdPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.THIRD_PROPERTY_NAME, thirdPropertyNameToken.TextSpan.GetText());

                var thirdPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.THIRD_PROPERTY_VALUE, thirdPropertyValueToken.TextSpan.GetText());

                var fourthPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FOURTH_PROPERTY_NAME, fourthPropertyNameToken.TextSpan.GetText());

                var fourthPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FOURTH_PROPERTY_VALUE, fourthPropertyValueToken.TextSpan.GetText());

                var endToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endToken.TextSpan.GetText());
            }

            // GlobalSection(SolutionProperties)
            {
                var startToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());

                var startParameterToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionSolutionProperties.START_TOKEN, startParameterToken.TextSpan.GetText());

                var startOrderToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionProperties.START_TOKEN_ORDER, startOrderToken.TextSpan.GetText());

                var solutionGuidNameToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionSolutionProperties.HIDE_SOLUTION_NODE_NAME, solutionGuidNameToken.TextSpan.GetText());

                var solutionGuidValueToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionProperties.HIDE_SOLUTION_NODE_VALUE, solutionGuidValueToken.TextSpan.GetText());

                var endToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endToken.TextSpan.GetText());
            }

            // NestedProjects
            {
                var startToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());

                var startParameterToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionNestedProjects.START_TOKEN, startParameterToken.TextSpan.GetText());

                var startOrderToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionNestedProjects.START_TOKEN_ORDER, startOrderToken.TextSpan.GetText());

                var firstPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionNestedProjects.FIRST_PROPERTY_NAME, firstPropertyNameToken.TextSpan.GetText());

                var firstPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionNestedProjects.FIRST_PROPERTY_VALUE, firstPropertyValueToken.TextSpan.GetText());

                var secondPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionNestedProjects.SECOND_PROPERTY_NAME, secondPropertyNameToken.TextSpan.GetText());

                var secondPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionNestedProjects.SECOND_PROPERTY_VALUE, secondPropertyValueToken.TextSpan.GetText());

                var endToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endToken.TextSpan.GetText());
            }

            // GlobalSection(ExtensibilityGlobals)
            {
                var startToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startToken.TextSpan.GetText());

                var startParameterToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionExtensibilityGlobals.START_TOKEN, startParameterToken.TextSpan.GetText());

                var startOrderToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.START_TOKEN_ORDER, startOrderToken.TextSpan.GetText());

                var firstPropertyNameToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.FIRST_PROPERTY_NAME, firstPropertyNameToken.TextSpan.GetText());

                var firstPropertyValueToken = (IdentifierToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.FIRST_PROPERTY_VALUE, firstPropertyValueToken.TextSpan.GetText());

                var endToken = (KeywordToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endToken.TextSpan.GetText());
            }

            var globalEndToken = (KeywordToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Global.END_TOKEN, globalEndToken.TextSpan.GetText());
        }

        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[i++];
        Assert.NotNull(endOfFileToken);
    }
}
