using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record BinaryOperatorNodeTests
{
	[Fact]
	public void BinaryOperatorNode()
	{
		//public BinaryOperatorNode(
	 //       TypeClauseNode leftOperandTypeClauseNode,
	 //       ISyntaxToken operatorToken,
	 //       TypeClauseNode rightOperandTypeClauseNode,
	 //       TypeClauseNode typeClauseNode)
	}

	[Fact]
	public void LeftOperandTypeClauseNode()
	{
		//public TypeClauseNode LeftOperandTypeClauseNode { get; }
	}

	[Fact]
	public void OperatorToken()
	{
		//public ISyntaxToken OperatorToken { get; }
	}

	[Fact]
	public void RightOperandTypeClauseNode()
	{
		//public TypeClauseNode RightOperandTypeClauseNode { get; }
	}

	[Fact]
	public void TypeClauseNode()
	{
		//public TypeClauseNode TypeClauseNode { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.BinaryOperatorNode;
	}
}