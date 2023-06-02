using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.ParserCase;

public partial class ParserTests
{
    [Fact]
    public void SHOULD_NOT_PARSE_COMMENT_SINGLE_LINE_STATEMENT()
    {
        string sourceText = @"// C:\Users\hunte\Repos\Aaa\"
            .ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var parser = new Parser(
            lexer.SyntaxTokens,
            lexer.Diagnostics);

        var compilationUnit = parser.Parse();

        Assert.Empty(compilationUnit.Children);
    }
}
