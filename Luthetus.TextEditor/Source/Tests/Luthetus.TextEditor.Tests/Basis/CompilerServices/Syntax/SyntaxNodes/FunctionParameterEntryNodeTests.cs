using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record FunctionParameterEntryNodeTests
{
	[Fact]
	public void FunctionParameterEntryNode()
	{
		//public FunctionParameterEntryNode(
		//	IExpressionNode expressionNode,
		//	bool hasOutKeyword,
		//	bool hasInKeyword,
		//	bool hasRefKeyword)
	}

	[Fact]
	public void ExpressionNode()
	{
		//public IExpressionNode ExpressionNode { get; }
	}
	
	[Fact]
	public void HasOutKeyword()
	{
		//public bool HasOutKeyword { get; }
	}

	[Fact]
	public void HasInKeyword()
	{
		//public bool HasInKeyword { get; }
	}

	[Fact]
	public void HasRefKeyword()
	{
		//public bool HasRefKeyword { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.FunctionParameterEntryNode;
	}
}