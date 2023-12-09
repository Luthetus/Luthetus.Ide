using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record AmbiguousIdentifierNodeTests
{
	[Fact]
	public void AmbiguousIdentifierNode()
	{
		//public AmbiguousIdentifierNode(IdentifierToken identifierToken)
	}

    [Fact]
	public void IdentifierToken()
	{
		//public IdentifierToken IdentifierToken { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.AmbiguousIdentifierNode;
	}
}
