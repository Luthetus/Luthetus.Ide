using Luthetus.CompilerServices.Lang.DotNetSolution.Code;
using Luthetus.CompilerServices.Lang.DotNetSolution.Facts;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.Tests.Parser;

public class HeaderDoesParse
{
    [Fact]
    public void FULL_DOES_PARSE()
    {
        var formatVersionValue = "12.00";
        var hashtagVersionValue = "17";
        var exactVersionValue = "17.7.34018.315";
        var minimumVersionValue = "10.0.40219.1";

        var lexer = new TestDotNetSolutionLexer(
            new(string.Empty),
            TestDataHeader.FULL);

        lexer.Lex();

        var parser = new TestDotNetSolutionParser(lexer);
        var compilationUnit = parser.Parse();
    }
}