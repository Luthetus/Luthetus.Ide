using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record NamespaceStatementNodeTests
{
	[Fact]
	public void NamespaceStatementNode()
	{
		//public NamespaceStatementNode(
	 //       KeywordToken keywordToken,
	 //       IdentifierToken identifierToken,
	 //       ImmutableArray<NamespaceEntryNode> namespaceEntryNodes)
	}

	[Fact]
	public void KeywordToken()
	{
		//public KeywordToken KeywordToken { get; }
	}

	[Fact]
	public void IdentifierToken()
	{
		//public IdentifierToken IdentifierToken { get; }
	}

	[Fact]
	public void NamespaceEntryNodeBag()
	{
		//public ImmutableArray<NamespaceEntryNode> NamespaceEntryNodeBag { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.NamespaceStatementNode;
	}

	[Fact]
	public void GetTopLevelTypeDefinitionNodes()
	{
		//public ImmutableArray<TypeDefinitionNode> GetTopLevelTypeDefinitionNodes()
	}
}