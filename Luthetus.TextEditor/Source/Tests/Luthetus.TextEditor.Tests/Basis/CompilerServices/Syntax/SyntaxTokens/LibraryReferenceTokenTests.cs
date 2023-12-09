using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

public sealed record LibraryReferenceTokenTests
{
	[Fact]
	public void LibraryReferenceToken()
	{
		//public LibraryReferenceToken(TextEditorTextSpan textSpan, bool isAbsolutePath)
	}

	[Fact]
	public void TextSpan()
	{
		//public TextEditorTextSpan TextSpan { get; }
	}

	[Fact]
	public void SyntaxKind()
	{
		//public SyntaxKind SyntaxKind => SyntaxKind.LibraryReferenceToken;
	}

	[Fact]
	public void IsAbsolutePath()
	{
		//public bool IsAbsolutePath { get; }
	}

	[Fact]
	public void IsFabricated()
	{
		//public bool IsFabricated { get; init; }
	}
}