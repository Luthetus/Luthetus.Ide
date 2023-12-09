using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record ConstraintNodeTests
{
	[Fact]
	public void ConstraintNode()
	{
		//public ConstraintNode(ImmutableArray<ISyntaxToken> innerTokens)
	}


	[Fact]
	public void InnerTokens()
	{
		//public ImmutableArray<ISyntaxToken> InnerTokens { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.TypeClauseNode;
	}
}