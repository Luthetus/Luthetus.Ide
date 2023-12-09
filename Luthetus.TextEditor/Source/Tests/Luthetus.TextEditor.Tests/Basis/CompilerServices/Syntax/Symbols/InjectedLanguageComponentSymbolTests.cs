using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

public sealed record InjectedLanguageComponentSymbolTests
{
	[Fact]
	public void InjectedLanguageComponentSymbol()
	{
		//public InjectedLanguageComponentSymbol(TextEditorTextSpan textSpan)
	}

	[Fact]
	public void TextSpan()
	{
		//public TextEditorTextSpan TextSpan { get; init; }
	}

	[Fact]
	public void SymbolKindString()
	{
		//public string SymbolKindString => SyntaxKind.ToString();
	}

	[Fact]
	public void SyntaxKind()
	{
		//public SyntaxKind SyntaxKind => SyntaxKind.InjectedLanguageComponentSymbol;
	}
}