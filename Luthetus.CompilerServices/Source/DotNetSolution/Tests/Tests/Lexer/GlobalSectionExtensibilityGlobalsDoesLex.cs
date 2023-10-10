using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Lexer;

public class GlobalSectionExtensibilityGlobalsDoesLex
{
    [Fact]
    public void START_TOKEN_DOES_LEX()
    {
        var sourceText = $"{LexSolutionFacts.GlobalSection.START_TOKEN}({LexSolutionFacts.GlobalSectionExtensibilityGlobals.START_TOKEN})";

        var lexer = new DotNetSolutionLexer(
            new(string.Empty),
            sourceText);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(2, lexer.SyntaxTokens.Length);

        var i = 0;

        var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
        var startAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];

        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());
        Assert.Equal(LexSolutionFacts.GlobalSectionExtensibilityGlobals.START_TOKEN, startAssociatedValueToken.TextSpan.GetText());
    }

    [Fact]
    public void FULL_DOES_LEX()
    {
        var lexer = new DotNetSolutionLexer(
            new(string.Empty),
            TestDataGlobalSectionExtensibilityGlobals.FULL);

        lexer.LexGlobalSection(() => false);

        Assert.Equal(6, lexer.SyntaxTokens.Length);

        var i = 0;

        var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[i++];
        Assert.Equal(LexSolutionFacts.GlobalSection.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());

        var startParameterAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(LexSolutionFacts.GlobalSectionExtensibilityGlobals.START_TOKEN, startParameterAssociatedValueToken.TextSpan.GetText());

        var startOrderAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.START_TOKEN_ORDER, startOrderAssociatedValueToken.TextSpan.GetText());

        var firstPropertyNameAssociatedNameToken = (AssociatedNameToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.FIRST_PROPERTY_NAME, firstPropertyNameAssociatedNameToken.TextSpan.GetText());

        var firstPropertyValueAssociatedValueToken = (AssociatedValueToken)lexer.SyntaxTokens[i++];
        Assert.Equal(TestDataGlobalSectionExtensibilityGlobals.FIRST_PROPERTY_VALUE, firstPropertyValueAssociatedValueToken.TextSpan.GetText());

        var endCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[i++];
        Assert.Equal(LexSolutionFacts.GlobalSection.END_TOKEN, endCloseAssociatedGroupToken.TextSpan.GetText());
    }
}