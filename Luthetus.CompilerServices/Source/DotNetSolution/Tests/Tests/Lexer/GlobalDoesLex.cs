using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.CompilerServices.Lang.DotNetSolution.Code;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Lexer;

public class GlobalDoesLex
{
    [Fact]
    public void START_TOKEN_DOES_LEX()
    {
        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            LexSolutionFacts.Global.START_TOKEN);

        lexer.Lex();

        Assert.Equal(2, lexer.SyntaxTokens.Length);

        var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[0];
        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[1];

        Assert.Equal(LexSolutionFacts.Global.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());
        Assert.NotNull(endOfFileToken);
    }

    /// <summary>
    /// A real .sln has data betwen the 'Global' and 'EndGlobal' delimiters.
    /// This test just ensures the two delimiters are recognized since there is no data
    /// between them for this test.
    /// </summary>
    [Fact]
    public void FULL_DOES_LEX()
    {
        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataGlobal.FULL);

        lexer.Lex();

        var startOpenAssociatedGroupToken = (OpenAssociatedGroupToken)lexer.SyntaxTokens[0];
        Assert.Equal(LexSolutionFacts.Global.START_TOKEN, startOpenAssociatedGroupToken.TextSpan.GetText());

        var endCloseAssociatedGroupToken = (CloseAssociatedGroupToken)lexer.SyntaxTokens[1];
        Assert.Equal(LexSolutionFacts.Global.END_TOKEN, endCloseAssociatedGroupToken.TextSpan.GetText());

        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[2];
        Assert.NotNull(endOfFileToken);
    }
}