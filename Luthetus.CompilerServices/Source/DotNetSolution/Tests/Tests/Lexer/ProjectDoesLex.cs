using Luthetus.CompilerServices.Lang.DotNetSolution.Code;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Lexer;

public class ProjectDoesLex
{
    [Fact]
    public void START_TOKEN_DOES_LEX()
    {
        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN);

        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokens.Length);

        var startProjectDefinitionKeywordToken = (KeywordToken)lexer.SyntaxTokens[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

        Assert.Equal(LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN, startProjectDefinitionKeywordToken.TextSpan.GetText());
        Assert.NotNull(endOfFileToken);
    }

    [Fact]
    public void GUID_DOES_LEX()
    {
        var guidValue = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataProject.GUID);

        lexer.LexGuid();

        Assert.Single(lexer.SyntaxTokens);

        var guidIdentifierToken = (IdentifierToken)lexer.SyntaxTokens.Single();

        Assert.Equal(guidValue, guidIdentifierToken.TextSpan.GetText());
    }

    [Fact]
    public void NAME_NO_EXTENSION_DOES_LEX()
    {
        var nameNoExtensionValue = "ConsoleApp2";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataProject.NAME_NO_EXTENSION);

        lexer.LexString();

        Assert.Single(lexer.SyntaxTokens);

        var nameNoExtensionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens.Single();

        Assert.Equal(nameNoExtensionValue, nameNoExtensionIdentifierToken.TextSpan.GetText());
    }

    [Fact]
    public void RELATIVE_PATH_FROM_SLN_DOES_LEX()
    {
        var relativePathFromSlnValue = "ConsoleApp2\\ConsoleApp2.csproj";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataProject.RELATIVE_PATH_FROM_SLN);

        lexer.LexString();

        Assert.Single(lexer.SyntaxTokens);

        var relativePathFromSlnIdentifierToken = (IdentifierToken)lexer.SyntaxTokens.Single();

        Assert.Equal(relativePathFromSlnValue, relativePathFromSlnIdentifierToken.TextSpan.GetText());
    }

    [Fact]
    public void FULL_DOES_LEX()
    {
        var projectTypeGuidValue = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
        var nameNoExtensionValue = "ConsoleApp2";
        var relativePathFromSlnValue = "ConsoleApp2\\ConsoleApp2.csproj";
        var projectIdGuidValue = "623099D9-D9DE-47E8-8CCD-BC301C82F70F";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataProject.FULL);

        lexer.Lex();

        Assert.Equal(7, lexer.SyntaxTokens.Length);

        var startProjectDefinitionKeywordToken = (KeywordToken)lexer.SyntaxTokens[0];
        Assert.Equal(LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN, startProjectDefinitionKeywordToken.TextSpan.GetText());

        var projectTypeGuidIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[1];
        Assert.Equal(projectTypeGuidValue, projectTypeGuidIdentifierToken.TextSpan.GetText());

        var nameNoExtensionIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[2];
        Assert.Equal(nameNoExtensionValue, nameNoExtensionIdentifierToken.TextSpan.GetText());

        var relativePathFromSlnIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[3];
        Assert.Equal(relativePathFromSlnValue, relativePathFromSlnIdentifierToken.TextSpan.GetText());

        var projectIdGuidIdentifierToken = (IdentifierToken)lexer.SyntaxTokens[4];
        Assert.Equal(projectIdGuidValue, projectIdGuidIdentifierToken.TextSpan.GetText());

        var endProjectDefinitionKeywordToken = (KeywordToken)lexer.SyntaxTokens[5];
        Assert.Equal(LexSolutionFacts.Project.PROJECT_DEFINITION_END_TOKEN, endProjectDefinitionKeywordToken.TextSpan.GetText());

        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[6];
        Assert.NotNull(endOfFileToken);
    }
}