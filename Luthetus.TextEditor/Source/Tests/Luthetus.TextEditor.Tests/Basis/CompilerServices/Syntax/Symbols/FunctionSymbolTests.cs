using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="FunctionSymbol"/>
/// </summary>
public class FunctionSymbolTests
{
    /// <summary>
    /// <see cref="FunctionSymbol(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="FunctionSymbol.TextSpan"/>
    /// <see cref="FunctionSymbol.SymbolKindString"/>
    /// <see cref="FunctionSymbol.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var textSpan = TextEditorTextSpan.FabricateTextSpan("MyMethod");
        var functionSymbol = new FunctionSymbol(textSpan);

        Assert.Equal(textSpan, functionSymbol.TextSpan);
        Assert.Equal(SyntaxKind.FunctionSymbol, functionSymbol.SyntaxKind);
        Assert.Equal(functionSymbol.SyntaxKind.ToString(), functionSymbol.SymbolKindString);
	}
}
