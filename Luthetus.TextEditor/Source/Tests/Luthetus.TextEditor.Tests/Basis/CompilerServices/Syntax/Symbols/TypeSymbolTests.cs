using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

public sealed record TypeSymbolTests
{
	[Fact]
	public void TypeSymbol()
	{
		//public TypeSymbol(TextEditorTextSpan textSpan)
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
		//public SyntaxKind SyntaxKind => SyntaxKind.TypeSymbol;
	}
}