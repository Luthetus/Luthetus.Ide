using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Parser;

public class GlobalDoesParse
{
    [Fact]
    public void FULL_DOES_PARSE()
    {
        var lexer = new DotNetSolutionLexer(
            new(string.Empty),
            TestDataGlobal.FULL);

        lexer.Lex();

        var startKeywordToken = (KeywordToken)lexer.SyntaxTokens[0];
        Assert.Equal(LexSolutionFacts.Global.START_TOKEN, startKeywordToken.TextSpan.GetText());

        var endKeywordToken = (KeywordToken)lexer.SyntaxTokens[1];
        Assert.Equal(LexSolutionFacts.Global.END_TOKEN, endKeywordToken.TextSpan.GetText());

        var endOfFileToken = (EndOfFileToken)lexer.SyntaxTokens[2];
        Assert.NotNull(endOfFileToken);
    }
}