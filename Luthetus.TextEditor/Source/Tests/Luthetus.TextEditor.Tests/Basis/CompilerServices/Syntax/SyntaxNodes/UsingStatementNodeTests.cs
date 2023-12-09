using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record UsingStatementNodeTests
{
	[Fact]
	public void UsingStatementNode()
	{
		//public UsingStatementNode(KeywordToken keywordToken, IdentifierToken namespaceIdentifier)
	}

	[Fact]
	public void KeywordToken()
	{
		//public KeywordToken KeywordToken { get; }
	}

	[Fact]
	public void NamespaceIdentifier()
	{
		//public IdentifierToken NamespaceIdentifier { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.UsingStatementNode;
	}
}