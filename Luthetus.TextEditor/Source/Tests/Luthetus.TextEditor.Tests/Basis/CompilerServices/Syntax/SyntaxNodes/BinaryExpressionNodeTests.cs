using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record BinaryExpressionNodeTests
{
	[Fact]
	public void BinaryExpressionNode()
	{
		//public BinaryExpressionNode(
	 //       IExpressionNode leftExpressionNode,
	 //       BinaryOperatorNode binaryOperatorNode,
	 //       IExpressionNode rightExpressionNode)
	}


	[Fact]
	public void LeftExpressionNode()
	{
		//public IExpressionNode LeftExpressionNode { get; }
	}

	[Fact]
	public void BinaryOperatorNode()
	{
		//public BinaryOperatorNode BinaryOperatorNode { get; }
	}

	[Fact]
	public void RightExpressionNode()
	{
		//public IExpressionNode RightExpressionNode { get; }
	}

	[Fact]
	public void TypeClauseNode()
	{
		//public TypeClauseNode TypeClauseNode => BinaryOperatorNode.TypeClauseNode;
	}

	[Fact]
	public void ChildBag()
	{
		//public ImmutableArray<ISyntax> ChildBag { get; init; }
	}

	[Fact]
	public void IsFabricated()
	{
		//public bool IsFabricated { get; init; }
	}

	[Fact]
	public void SyntaxKind()
	{
		//public SyntaxKind SyntaxKind => SyntaxKind.BinaryExpressionNode;
	}
}