using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record VariableAssignmentExpressionNodeTests
{
	[Fact]
	public void VariableAssignmentExpressionNode()
	{
		//public VariableAssignmentExpressionNode(
	 //       IdentifierToken variableIdentifierToken,
	 //       EqualsToken equalsToken,
	 //       IExpressionNode expressionNode)
	}

	[Fact]
	public void VariableIdentifierToken()
	{
		//public IdentifierToken VariableIdentifierToken { get; }
	}

	[Fact]
	public void EqualsToken()
	{
		//public EqualsToken EqualsToken { get; }
	}

	[Fact]
	public void ExpressionNode()
	{
		//public IExpressionNode ExpressionNode { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.VariableAssignmentExpressionNode;
	}
}
