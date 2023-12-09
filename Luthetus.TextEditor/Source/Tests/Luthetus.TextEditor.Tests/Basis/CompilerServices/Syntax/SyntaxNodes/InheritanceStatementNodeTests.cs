using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record InheritanceStatementNodeTests
{
	[Fact]
	public void InheritanceStatementNode()
	{
		//public InheritanceStatementNode(TypeClauseNode parentTypeClauseNode)
	}

	[Fact]
	public void ParentTypeClauseNode()
	{
		//public TypeClauseNode ParentTypeClauseNode { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.InheritanceStatementNode;
	}
}