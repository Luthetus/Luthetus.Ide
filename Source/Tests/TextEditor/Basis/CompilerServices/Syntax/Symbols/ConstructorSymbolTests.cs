using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

/// <summary>
/// <see cref="ConstructorSymbol"/>
/// </summary>
public class ConstructorSymbolTests
{
    /// <summary>
    /// <see cref="ConstructorSymbol(TextEditorTextSpan)"/>
	/// <br/>----<br/>
    /// <see cref="ConstructorSymbol.TextSpan"/>
    /// <see cref="ConstructorSymbol.SymbolKindString"/>
    /// <see cref="ConstructorSymbol.SyntaxKind"/>
	/// </summary>
    [Fact]
	public void Constructor()
	{
		var textSpan = TextEditorTextSpan.FabricateTextSpan("Person");
        var constructorSymbol = new ConstructorSymbol(textSpan);

		Assert.Equal(textSpan, constructorSymbol.TextSpan);
		Assert.Equal(SyntaxKind.ConstructorSymbol, constructorSymbol.SyntaxKind);
		Assert.Equal(constructorSymbol.SyntaxKind.ToString(), constructorSymbol.SymbolKindString);
	}
}
