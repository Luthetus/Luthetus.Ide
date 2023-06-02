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
    public void SHOULD_LEX_PREPROCESSOR_DIRECTIVE_TOKEN()
    {
        var preprocessorDirectiveAsString = "#region regionIdentifierHere";
        var sourceText = $"{preprocessorDirectiveAsString}".ReplaceLineEndings("\n");

        var resourceUri = new ResourceUri(string.Empty);

        var lexer = new Lexer(
            resourceUri,
            sourceText);

        lexer.Lex();

        var preprocessorDirectiveToken = lexer.SyntaxTokens.First();

        Assert.Equal(SyntaxKind.PreprocessorDirectiveToken, preprocessorDirectiveToken.SyntaxKind);

        var text = preprocessorDirectiveToken.TextSpan.GetText();
        Assert.Equal(preprocessorDirectiveAsString, text);
    }
}
