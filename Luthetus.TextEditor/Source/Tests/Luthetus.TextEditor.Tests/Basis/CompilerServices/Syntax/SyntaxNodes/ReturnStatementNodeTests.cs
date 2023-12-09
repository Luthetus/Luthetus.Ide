using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record ReturnStatementNodeTests
{
	[Fact]
	public void ReturnStatementNode()
	{
		//public ReturnStatementNode(KeywordToken keywordToken, IExpressionNode expressionNode)
	}

	[Fact]
	public void KeywordToken()
	{
		//public KeywordToken KeywordToken { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.ReturnStatementNode;
	}
}