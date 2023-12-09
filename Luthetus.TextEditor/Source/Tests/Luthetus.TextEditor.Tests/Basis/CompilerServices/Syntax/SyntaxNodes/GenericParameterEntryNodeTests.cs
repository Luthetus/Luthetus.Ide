using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record GenericParameterEntryNodeTests
{
	[Fact]
	public void GenericParameterEntryNode()
	{
		//public GenericParameterEntryNode(TypeClauseNode typeClauseNode)
	}

	[Fact]
	public void TypeClauseNode()
	{
		//public TypeClauseNode TypeClauseNode { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.GenericParameterEntryNode;
	}
}