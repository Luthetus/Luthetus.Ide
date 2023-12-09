using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record GenericParametersListingNodeTests
{
	[Fact]
	public void GenericParametersListingNode()
	{
		//public GenericParametersListingNode(
		//	OpenAngleBracketToken openAngleBracketToken,
		//	ImmutableArray<GenericParameterEntryNode> genericParameterEntryNodes,
		//	CloseAngleBracketToken closeAngleBracketToken)
	}

	[Fact]
	public void OpenAngleBracketToken()
	{
		//public OpenAngleBracketToken OpenAngleBracketToken { get; }
	}

	[Fact]
	public void GenericParameterEntryNodeBag()
	{
		//public ImmutableArray<GenericParameterEntryNode> GenericParameterEntryNodeBag { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.GenericParametersListingNode;
	}
}