using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record FunctionArgumentsListingNodeTests
{
	[Fact]
	public void FunctionArgumentsListingNode()
	{
		//public FunctionArgumentsListingNode(
		//	OpenParenthesisToken openParenthesisToken,
		//	ImmutableArray<FunctionArgumentEntryNode> functionArgumentEntryNodes,
		//	CloseParenthesisToken closeParenthesisToken)
	}

	[Fact]
	public void OpenParenthesisToken()
	{
		//public OpenParenthesisToken OpenParenthesisToken { get; }
	}

	[Fact]
	public void FunctionArgumentEntryNodeBag()
	{
		//public ImmutableArray<FunctionArgumentEntryNode> FunctionArgumentEntryNodeBag { get; }
	}

	[Fact]
	public void CloseParenthesisToken()
	{
		//public CloseParenthesisToken CloseParenthesisToken { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentsListingNode;
	}
}