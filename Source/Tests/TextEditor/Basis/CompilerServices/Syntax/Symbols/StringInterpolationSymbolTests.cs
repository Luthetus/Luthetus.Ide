using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="StringInterpolationSymbol"/>
/// </summary>
public class StringInterpolationSymbolTests
{
    /// <summary>
    /// <see cref="StringInterpolationSymbol(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="StringInterpolationSymbol.TextSpan"/>
    /// <see cref="StringInterpolationSymbol.SymbolKindString"/>
    /// <see cref="StringInterpolationSymbol.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var textSpan = TextEditorTextSpan.FabricateTextSpan("$");
        var stringInterpolationSymbol = new StringInterpolationSymbol(textSpan);

        Assert.Equal(textSpan, stringInterpolationSymbol.TextSpan);
        Assert.Equal(SyntaxKind.StringInterpolationSymbol, stringInterpolationSymbol.SyntaxKind);
        Assert.Equal(stringInterpolationSymbol.SyntaxKind.ToString(), stringInterpolationSymbol.SymbolKindString);
    }
}