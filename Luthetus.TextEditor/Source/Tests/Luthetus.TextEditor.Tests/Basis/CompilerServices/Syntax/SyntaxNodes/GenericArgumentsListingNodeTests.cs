using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record GenericArgumentsListingNodeTests
{
	[Fact]
	public void GenericArgumentsListingNode()
	{
		//public GenericArgumentsListingNode(
	 //       OpenAngleBracketToken openAngleBracketToken,
	 //       ImmutableArray<GenericArgumentEntryNode> genericArgumentEntryNodes,
	 //       CloseAngleBracketToken closeAngleBracketToken)
	}

	[Fact]
	public void OpenAngleBracketToken()
	{
		//public OpenAngleBracketToken OpenAngleBracketToken { get; }
	}

	[Fact]
	public void GenericArgumentEntryNodeBag()
	{
		//public ImmutableArray<GenericArgumentEntryNode> GenericArgumentEntryNodeBag { get; }
	}

	[Fact]
	public void CloseAngleBracketToken()
	{
		//public CloseAngleBracketToken CloseAngleBracketToken { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.GenericArgumentsListingNode;
	}
}