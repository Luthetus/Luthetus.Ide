using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.LexerCase;

public partial class LexerTests
{
    [Fact]
    public void SHOULD_LEX_KEYWORD_TOKEN()
    {
        var keywordAsString = "int";
        var sourceText = $"{keywordAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var keywordToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.KeywordToken, keywordToken.SyntaxKind);

        var text = keywordToken.TextSpan.GetText();
        Assert.Equal(keywordAsString, text);
    }
}
