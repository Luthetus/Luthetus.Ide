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

        var i = 0;

        var startProjectDefinitionOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN, startProjectDefinitionOpenAssociatedGroupToken.TextSpan.GetText());
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

        var guidAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens.Single();

        Assert.Equal(guidValue, guidAssociatedValueToken.TextSpan.GetText());
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

        var nameNoExtensionAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens.Single();

        Assert.Equal(nameNoExtensionValue, nameNoExtensionAssociatedValueToken.TextSpan.GetText());
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

        var relativePathFromSlnAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens.Single();

        Assert.Equal(relativePathFromSlnValue, relativePathFromSlnAssociatedValueToken.TextSpan.GetText());
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

        var i = 0;

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

        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[i++];
        Assert.NotNull(endOfFileToken);
    }
}