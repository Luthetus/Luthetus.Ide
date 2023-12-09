using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record UnaryOperatorNodeTests
{
	[Fact]
	public void UnaryOperatorNode()
	{
		//public UnaryOperatorNode(
	 //       TypeClauseNode operandTypeClauseNode,
	 //       ISyntaxToken operatorToken,
	 //       TypeClauseNode resultTypeClauseNode)
	}

	[Fact]
	public void OperandTypeClauseNode()
	{
		//public TypeClauseNode OperandTypeClauseNode { get; }
	}

	[Fact]
	public void OperatorToken()
	{
		//public ISyntaxToken OperatorToken { get; }
	}

	[Fact]
	public void ResultTypeClauseNode()
	{
		//public TypeClauseNode ResultTypeClauseNode { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.UnaryOperatorNode;
	}
}