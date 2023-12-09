using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

public sealed record PropertySymbolTests
{
	[Fact]
	public void PropertySymbol()
	{
		//public PropertySymbol(TextEditorTextSpan textSpan)
	}
    
	[Fact]
	public void TextSpan()
	{
		//public TextEditorTextSpan TextSpan { get; }
	}

	[Fact]
	public void SymbolKindString()
	{
		//public string SymbolKindString => SyntaxKind.ToString();
	}

	[Fact]
	public void SyntaxKind()
	{
		//public SyntaxKind SyntaxKind => SyntaxKind.PropertySymbol;
	}
}
