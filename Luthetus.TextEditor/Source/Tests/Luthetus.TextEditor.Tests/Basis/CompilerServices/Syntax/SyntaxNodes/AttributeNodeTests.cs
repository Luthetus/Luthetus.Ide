using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record AttributeNodeTests
{
	[Fact]
	public void AttributeNode()
	{
		//public AttributeNode(
	 //       OpenSquareBracketToken openSquareBracketToken,
	 //       CloseSquareBracketToken closeSquareBracketToken)
	}

	[Fact]
	public void OpenSquareBracketToken()
	{
		//public OpenSquareBracketToken OpenSquareBracketToken { get; }
	}

	[Fact]
	public void CloseSquareBracketToken()
	{
		//public CloseSquareBracketToken CloseSquareBracketToken { get; }
	}

	[Fact]
	public void ChildBag()
	{
		//public ImmutableArray<ISyntax> ChildBag { get; }
	}

	[Fact]
	public void IsFabricated()
	{
		//public bool IsFabricated { get; init; }
	}

	[Fact]
	public void SyntaxKind()
	{
		//public SyntaxKind SyntaxKind => SyntaxKind.AttributeNode;
	}
}