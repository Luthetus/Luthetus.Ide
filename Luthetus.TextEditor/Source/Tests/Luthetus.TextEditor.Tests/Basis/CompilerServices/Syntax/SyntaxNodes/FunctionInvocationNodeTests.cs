using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record FunctionInvocationNodeTests
{
	[Fact]
	public void FunctionInvocationNode()
	{
		//public FunctionInvocationNode(
		//	IdentifierToken functionInvocationIdentifierToken,
		//	FunctionDefinitionNode? functionDefinitionNode,
		//	GenericParametersListingNode? genericParametersListingNode,
		//	FunctionParametersListingNode functionParametersListingNode)
	}

	[Fact]
	public void FunctionInvocationIdentifierToken()
	{
		//public IdentifierToken FunctionInvocationIdentifierToken { get; }
	}

	[Fact]
	public void FunctionDefinitionNode()
	{
		//public FunctionDefinitionNode? FunctionDefinitionNode { get; }
	}

	[Fact]
	public void GenericParametersListingNode()
	{
		//public GenericParametersListingNode? GenericParametersListingNode { get; }
	}

	[Fact]
	public void FunctionParametersListingNode()
	{
		//public FunctionParametersListingNode FunctionParametersListingNode { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.FunctionInvocationNode;
	}
}