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
        }

        // Project
        {
            var projectTypeGuidValue = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
            var nameNoExtensionValue = "ConsoleApp2";
            var relativePathFromSlnValue = "ConsoleApp2\\ConsoleApp2.csproj";
            var projectIdGuidValue = "623099D9-D9DE-47E8-8CCD-BC301C82F70F";

            var startProjectDefinitionOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN, startProjectDefinitionOpenAssociatedGroupToken.TextSpan.GetText());

            var projectTypeGuidAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
            Assert.Equal(projectTypeGuidValue, projectTypeGuidAssociatedValueToken.TextSpan.GetText());

            var nameNoExtensionAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
            Assert.Equal(nameNoExtensionValue, nameNoExtensionAssociatedValueToken.TextSpan.GetText());

            var relativePathFromSlnAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
            Assert.Equal(relativePathFromSlnValue, relativePathFromSlnAssociatedValueToken.TextSpan.GetText());

            var projectIdGuidAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
            Assert.Equal(projectIdGuidValue, projectIdGuidAssociatedValueToken.TextSpan.GetText());

            var endProjectDefinitionCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Project.PROJECT_DEFINITION_END_TOKEN, endProjectDefinitionCloseAssociatedGroupToken.TextSpan.GetText());
        }

        // Global
        {
            var globalStartOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Global.START_TOKEN, globalStartOpenAssociatedGroupToken.TextSpan.GetText());

            // GlobalSection(SolutionConfigurationPlatforms)
            {
                var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());

                var startParameterAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionSolutionConfigurationPlatforms.START_TOKEN, startParameterAssociatedValueToken.TextSpan.GetText());

                var startOrderAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionConfigurationPlatforms.START_TOKEN_ORDER, startOrderAssociatedValueToken.TextSpan.GetText());

                var firstPropertyAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionConfigurationPlatforms.FIRST_PROPERTY_NAME, firstPropertyAssociatedNameToken.TextSpan.GetText());

                var firstPropertyAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionConfigurationPlatforms.FIRST_PROPERTY_VALUE, firstPropertyAssociatedValueToken.TextSpan.GetText());

                var secondPropertyAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionConfigurationPlatforms.SECOND_PROPERTY_NAME, secondPropertyAssociatedNameToken.TextSpan.GetText());

                var secondPropertyAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionConfigurationPlatforms.SECOND_PROPERTY_VALUE, secondPropertyAssociatedValueToken.TextSpan.GetText());

                var endCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endCloseAssociatedGroupToken.TextSpan.GetText());
            }

            // GlobalSection(ProjectConfigurationPlatforms)
            {
                var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());

                var startParameterAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionProjectConfigurationPlatforms.START_TOKEN, startParameterAssociatedValueToken.TextSpan.GetText());

                var startOrderAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.START_TOKEN_ORDER, startOrderAssociatedValueToken.TextSpan.GetText());

                var firstPropertyAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FIRST_PROPERTY_NAME, firstPropertyAssociatedNameToken.TextSpan.GetText());

                var firstPropertyAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FIRST_PROPERTY_VALUE, firstPropertyAssociatedValueToken.TextSpan.GetText());

                var secondPropertyAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.SECOND_PROPERTY_NAME, secondPropertyAssociatedNameToken.TextSpan.GetText());

                var secondPropertyAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.SECOND_PROPERTY_VALUE, secondPropertyAssociatedValueToken.TextSpan.GetText());

                var thirdPropertyAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.THIRD_PROPERTY_NAME, thirdPropertyAssociatedNameToken.TextSpan.GetText());

                var thirdPropertyAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.THIRD_PROPERTY_VALUE, thirdPropertyAssociatedValueToken.TextSpan.GetText());

                var fourthPropertyAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FOURTH_PROPERTY_NAME, fourthPropertyAssociatedNameToken.TextSpan.GetText());

                var fourthPropertyAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionProjectConfigurationPlatforms.FOURTH_PROPERTY_VALUE, fourthPropertyAssociatedValueToken.TextSpan.GetText());

                var endCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endCloseAssociatedGroupToken.TextSpan.GetText());
            }

            // GlobalSection(SolutionProperties)
            {
                var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());

                var startParameterAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionSolutionProperties.START_TOKEN, startParameterAssociatedValueToken.TextSpan.GetText());

                var startOrderAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionProperties.START_TOKEN_ORDER, startOrderAssociatedValueToken.TextSpan.GetText());

                var solutionGuidAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionSolutionProperties.HIDE_SOLUTION_NODE_NAME, solutionGuidAssociatedNameToken.TextSpan.GetText());

                var solutionGuidAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionSolutionProperties.HIDE_SOLUTION_NODE_VALUE, solutionGuidAssociatedValueToken.TextSpan.GetText());

                var endCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endCloseAssociatedGroupToken.TextSpan.GetText());
            }

            // NestedProjects
            {
                var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());

                var startParameterAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionNestedProjects.START_TOKEN, startParameterAssociatedValueToken.TextSpan.GetText());

                var startOrderAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionNestedProjects.START_TOKEN_ORDER, startOrderAssociatedValueToken.TextSpan.GetText());

                var firstPropertyAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionNestedProjects.FIRST_PROPERTY_NAME, firstPropertyAssociatedNameToken.TextSpan.GetText());

                var firstPropertyAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionNestedProjects.FIRST_PROPERTY_VALUE, firstPropertyAssociatedValueToken.TextSpan.GetText());

                var secondPropertyAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionNestedProjects.SECOND_PROPERTY_NAME, secondPropertyAssociatedNameToken.TextSpan.GetText());

                var secondPropertyAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionNestedProjects.SECOND_PROPERTY_VALUE, secondPropertyAssociatedValueToken.TextSpan.GetText());

                var endCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endCloseAssociatedGroupToken.TextSpan.GetText());
            }

            // GlobalSection(ExtensibilityGlobals)
            {
                var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());

                var startParameterAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSectionExtensibilityGlobals.START_TOKEN, startParameterAssociatedValueToken.TextSpan.GetText());

                var startOrderAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.START_TOKEN_ORDER, startOrderAssociatedValueToken.TextSpan.GetText());

                var firstPropertyAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.FIRST_PROPERTY_NAME, firstPropertyAssociatedNameToken.TextSpan.GetText());

                var firstPropertyAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
                Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.FIRST_PROPERTY_VALUE, firstPropertyAssociatedValueToken.TextSpan.GetText());

                var endCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[i++];
                Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endCloseAssociatedGroupToken.TextSpan.GetText());
            }

            var globalEndCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[i++];
            Assert.Equal(LexSolutionFacts.Global.END_TOKEN, globalEndCloseAssociatedGroupToken.TextSpan.GetText());
        }

        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[i++];
        Assert.NotNull(endOfFileToken);
    }
}
