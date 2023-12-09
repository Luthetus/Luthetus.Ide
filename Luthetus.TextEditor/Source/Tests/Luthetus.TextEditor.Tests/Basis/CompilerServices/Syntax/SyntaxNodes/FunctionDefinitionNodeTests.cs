using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record FunctionDefinitionNodeTests
{
	[Fact]
	public void FunctionDefinitionNode()
	{
		//public FunctionDefinitionNode(
		//	TypeClauseNode returnTypeClauseNode,
		//	IdentifierToken functionIdentifier,
		//	GenericArgumentsListingNode? genericArgumentsListingNode,
		//	FunctionArgumentsListingNode functionArgumentsListingNode,
		//	CodeBlockNode? functionBodyCodeBlockNode,
		//	ConstraintNode? constraintNode)
	}

	[Fact]
	public void ReturnTypeClauseNode()
	{
		//public TypeClauseNode ReturnTypeClauseNode { get; }
	}

	[Fact]
	public void FunctionIdentifier()
	{
		//public IdentifierToken FunctionIdentifier { get; }
	}

	[Fact]
	public void GenericArgumentsListingNode()
	{
		//public GenericArgumentsListingNode? GenericArgumentsListingNode { get; }
	}

	[Fact]
	public void FunctionArgumentsListingNode()
	{
		//public FunctionArgumentsListingNode FunctionArgumentsListingNode { get; }
	}

	[Fact]
	public void FunctionBodyCodeBlockNode()
	{
		//public CodeBlockNode? FunctionBodyCodeBlockNode { get; }
	}

	[Fact]
	public void ConstraintNode()
	{
		//public ConstraintNode? ConstraintNode { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.FunctionDefinitionNode;
	}
}
