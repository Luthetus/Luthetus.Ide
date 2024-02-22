using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="InjectedLanguageComponentSymbol"/>
/// </summary>
public class InjectedLanguageComponentSymbolTests
{
    /// <summary>
    /// <see cref="InjectedLanguageComponentSymbol(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="InjectedLanguageComponentSymbol.TextSpan"/>
    /// <see cref="InjectedLanguageComponentSymbol.SymbolKindString"/>
    /// <see cref="InjectedLanguageComponentSymbol.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var textSpan = TextEditorTextSpan.FabricateTextSpan("MyComponent");
        var injectedLanguageComponentSymbol = new InjectedLanguageComponentSymbol(textSpan);

        Assert.Equal(textSpan, injectedLanguageComponentSymbol.TextSpan);
        Assert.Equal(SyntaxKind.InjectedLanguageComponentSymbol, injectedLanguageComponentSymbol.SyntaxKind);
        Assert.Equal(injectedLanguageComponentSymbol.SyntaxKind.ToString(), injectedLanguageComponentSymbol.SymbolKindString);
    }
}