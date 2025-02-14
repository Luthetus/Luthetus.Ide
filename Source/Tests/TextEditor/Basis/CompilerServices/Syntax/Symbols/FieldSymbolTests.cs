using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="FieldSymbol"/>
/// </summary>
public class FieldSymbolTests
{
    /// <summary>
    /// <see cref="FieldSymbol(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="FieldSymbol.TextSpan"/>
    /// <see cref="FieldSymbol.SymbolKindString"/>
    /// <see cref="FieldSymbol.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var textSpan = TextEditorTextSpan.FabricateTextSpan("_id");
        var fieldSymbol = new FieldSymbol(textSpan);

        Assert.Equal(textSpan, fieldSymbol.TextSpan);
        Assert.Equal(SyntaxKind.FieldSymbol, fieldSymbol.SyntaxKind);
        Assert.Equal(fieldSymbol.SyntaxKind.ToString(), fieldSymbol.SymbolKindString);
	}
}