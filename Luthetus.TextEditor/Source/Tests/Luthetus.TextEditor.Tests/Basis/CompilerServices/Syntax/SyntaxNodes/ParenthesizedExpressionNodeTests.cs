using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record ParenthesizedExpressionNodeTests
{
	[Fact]
	public void ParenthesizedExpressionNode()
	{
		//public ParenthesizedExpressionNode(
		//	OpenParenthesisToken openParenthesisToken,
		//	IExpressionNode innerExpression,
		//	CloseParenthesisToken closeParenthesisToken)
	}

	[Fact]
	public void OpenParenthesisToken()
	{
		//public OpenParenthesisToken OpenParenthesisToken { get; }
	}

	[Fact]
	public void InnerExpression()
	{
		//public IExpressionNode InnerExpression { get; }
	}

	[Fact]
	public void CloseParenthesisToken()
	{
		//public CloseParenthesisToken CloseParenthesisToken { get; }
	}

	[Fact]
	public void TypeClauseNode()
	{
		//public TypeClauseNode TypeClauseNode => InnerExpression.TypeClauseNode;
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
		//public SyntaxKind SyntaxKind => SyntaxKind.ParenthesizedExpressionNode;
	}
}
