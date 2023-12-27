using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="PropertySymbol"/>
/// </summary>
public class PropertySymbolTests
{
    /// <summary>
    /// <see cref="PropertySymbol(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="PropertySymbol.TextSpan"/>
    /// <see cref="PropertySymbol.SymbolKindString"/>
    /// <see cref="PropertySymbol.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var textSpan = TextEditorTextSpan.FabricateTextSpan("PersonKey");
        var propertySymbol = new PropertySymbol(textSpan);

        Assert.Equal(textSpan, propertySymbol.TextSpan);
        Assert.Equal(SyntaxKind.PropertySymbol, propertySymbol.SyntaxKind);
        Assert.Equal(propertySymbol.SyntaxKind.ToString(), propertySymbol.SymbolKindString);
    }
}
