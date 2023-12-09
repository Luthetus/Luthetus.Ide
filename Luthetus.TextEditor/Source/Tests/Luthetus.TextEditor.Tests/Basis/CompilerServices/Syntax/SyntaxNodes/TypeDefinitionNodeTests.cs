using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record TypeDefinitionNodeTests
{
	[Fact]
	public void TypeDefinitionNode()
	{
		//public TypeDefinitionNode(
	 //       IdentifierToken typeIdentifier,
	 //       Type? valueType,
	 //       GenericArgumentsListingNode? genericArgumentsListingNode,
	 //       TypeClauseNode? inheritedTypeClauseNode,
	 //       CodeBlockNode? typeBodyCodeBlockNode)
	}

	[Fact]
	public void TypeIdentifier()
	{
		//public IdentifierToken TypeIdentifier { get; }
	}

	[Fact]
	public void ValueType()
	{
		//public Type? ValueType { get; }
	}

	[Fact]
	public void GenericArgumentsListingNode()
	{
		//public GenericArgumentsListingNode? GenericArgumentsListingNode { get; }
	}

	[Fact]
	public void InheritedTypeClauseNode()
	{
		//public TypeClauseNode? InheritedTypeClauseNode { get; }
	}

	[Fact]
	public void TypeBodyCodeBlockNode()
	{
		//public CodeBlockNode? TypeBodyCodeBlockNode { get; }
	}

	[Fact]
	public void IsInterface()
	{
		//public bool IsInterface { get; init; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.TypeDefinitionNode;
	}

	[Fact]
	public void GetFunctionDefinitionNodes()
	{
		//public ImmutableArray<FunctionDefinitionNode> GetFunctionDefinitionNodes()
	}

	[Fact]
	public void ToTypeClause()
	{
		//public TypeClauseNode ToTypeClause()
	}
}