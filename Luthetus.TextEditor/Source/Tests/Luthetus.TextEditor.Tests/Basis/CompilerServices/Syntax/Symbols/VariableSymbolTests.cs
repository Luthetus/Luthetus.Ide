using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.Symbols;

public sealed record VariableSymbolTests
{
	[Fact]
	public void VariableSymbol()
	{
		//public VariableSymbol(TextEditorTextSpan textSpan)
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
		//public SyntaxKind SyntaxKind => SyntaxKind.VariableSymbol;
	}
}