using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record TypeClauseNodeTests
{
	[Fact]
	public void TypeClauseNode()
	{
		//public TypeClauseNode(
	 //       ISyntaxToken typeIdentifier,
	 //       Type? valueType,
	 //       GenericParametersListingNode? genericParametersListingNode)
	}

	[Fact]
	public void TypeIdentifier()
	{
		//public ISyntaxToken TypeIdentifier { get; }
	}

	[Fact]
	public void ValueType()
	{
		//public Type? ValueType { get; }
	}

	[Fact]
	public void GenericParametersListingNode()
	{
		//public GenericParametersListingNode? GenericParametersListingNode { get; }
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