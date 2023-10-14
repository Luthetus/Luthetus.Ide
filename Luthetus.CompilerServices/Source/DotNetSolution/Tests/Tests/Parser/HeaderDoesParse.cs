using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;

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

        var lexer = new DotNetSolutionLexer(
            new(string.Empty),
            TestDataHeader.FULL);

        lexer.Lex();

        var parser = new DotNetSolutionParser(lexer);
        var compilationUnit = parser.Parse();
    }
}