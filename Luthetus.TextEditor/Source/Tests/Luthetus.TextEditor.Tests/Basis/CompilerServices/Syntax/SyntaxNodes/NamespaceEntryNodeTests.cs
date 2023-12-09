using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record NamespaceEntryNodeTests
{
	[Fact]
	public void NamespaceEntryNode()
	{
		//public NamespaceEntryNode(ResourceUri resourceUri, CodeBlockNode codeBlockNode)
	}

	[Fact]
	public void ResourceUri()
	{
		//public ResourceUri ResourceUri { get; }
	}

	[Fact]
	public void CodeBlockNode()
	{
		//public CodeBlockNode CodeBlockNode { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.NamespaceEntryNode;
	}
}