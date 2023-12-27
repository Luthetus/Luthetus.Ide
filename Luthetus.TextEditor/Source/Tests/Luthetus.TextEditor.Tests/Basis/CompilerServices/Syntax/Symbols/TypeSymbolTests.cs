using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="TypeSymbol"/>
/// </summary>
public class TypeSymbolTests
{
    /// <summary>
    /// <see cref="TypeSymbol(TextEditorTextSpan)"/>
    /// <see cref="TypeSymbol.TextSpan"/>
    /// <see cref="TypeSymbol.SymbolKindString"/>
    /// <see cref="TypeSymbol.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var textSpan = TextEditorTextSpan.FabricateTextSpan("$");
        var typeSymbol = new TypeSymbol(textSpan);

        Assert.Equal(textSpan, typeSymbol.TextSpan);
        Assert.Equal(SyntaxKind.TypeSymbol, typeSymbol.SyntaxKind);
        Assert.Equal(typeSymbol.SyntaxKind.ToString(), typeSymbol.SymbolKindString);
    }
}