using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record IfStatementNodeTests
{
	[Fact]
	public void IfStatementNode()
	{
		//public IfStatementNode(
	 //       KeywordToken keywordToken,
	 //       IExpressionNode expressionNode,
	 //       CodeBlockNode? ifStatementBodyCodeBlockNode)
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
	public void IfStatementBodyCodeBlockNode()
	{
		//public CodeBlockNode? IfStatementBodyCodeBlockNode { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.IfStatementNode;
	}
}