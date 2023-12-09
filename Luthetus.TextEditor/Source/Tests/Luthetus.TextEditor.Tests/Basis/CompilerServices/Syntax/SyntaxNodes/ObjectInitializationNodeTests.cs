using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record ObjectInitializationNodeTests
{
	[Fact]
	public void ObjectInitializationNode()
	{
		//public ObjectInitializationNode(OpenBraceToken openBraceToken, CloseBraceToken closeBraceToken)
	}

	[Fact]
	public void OpenBraceToken()
	{
		//public OpenBraceToken OpenBraceToken { get; }
	}

	[Fact]
	public void CloseBraceToken()
	{
		//public CloseBraceToken CloseBraceToken { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationNode;
	}
}