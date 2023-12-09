using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record UnaryExpressionNodeTests
{
	[Fact]
	public void UnaryExpressionNode()
	{
		//public UnaryExpressionNode(
	 //       IExpressionNode expression,
	 //       UnaryOperatorNode unaryOperatorNode)
	}

	[Fact]
	public void Expression()
	{
		//public IExpressionNode Expression { get; }
	}

	[Fact]
	public void UnaryOperatorNode()
	{
		//public UnaryOperatorNode UnaryOperatorNode { get; }
	}

	[Fact]
	public void TypeClauseNode()
	{
		//public TypeClauseNode TypeClauseNode => UnaryOperatorNode.ResultTypeClauseNode;
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
		//public SyntaxKind SyntaxKind => SyntaxKind.UnaryExpressionNode;
	}
}