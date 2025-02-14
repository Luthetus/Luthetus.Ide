using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="NamespaceSymbol"/>
/// </summary>
public class NamespaceSymbolTests
{
    /// <summary>
    /// <see cref="NamespaceSymbol(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="NamespaceSymbol.TextSpan"/>
    /// <see cref="NamespaceSymbol.SymbolKindString"/>
    /// <see cref="NamespaceSymbol.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var textSpan = TextEditorTextSpan.FabricateTextSpan("Luthetus");
        var namespaceSymbol = new NamespaceSymbol(textSpan);

        Assert.Equal(textSpan, namespaceSymbol.TextSpan);
        Assert.Equal(SyntaxKind.NamespaceSymbol, namespaceSymbol.SyntaxKind);
        Assert.Equal(namespaceSymbol.SyntaxKind.ToString(), namespaceSymbol.SymbolKindString);
    }
}