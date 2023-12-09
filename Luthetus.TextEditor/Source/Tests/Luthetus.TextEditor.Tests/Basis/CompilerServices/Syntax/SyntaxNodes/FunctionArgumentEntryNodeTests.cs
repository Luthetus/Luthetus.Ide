using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record FunctionArgumentEntryNodeTests
{
	[Fact]
	public void FunctionArgumentEntryNode()
	{
		//public FunctionArgumentEntryNode(
		//	VariableDeclarationNode variableDeclarationStatementNode,
		//	bool isOptional,
		//	bool hasOutKeyword,
		//	bool hasInKeyword,
		//	bool hasRefKeyword)
	}

	[Fact]
	public void VariableDeclarationStatementNode()
	{
		//public VariableDeclarationNode VariableDeclarationStatementNode { get; }
	}

	[Fact]
	public void IsOptional()
	{
		//public bool IsOptional { get; }
	}

	[Fact]
	public void HasOutKeyword()
	{
		//public bool HasOutKeyword { get; }
	}

	[Fact]
	public void HasInKeyword()
	{
		//public bool HasInKeyword { get; }
	}

	[Fact]
	public void HasRefKeyword()
	{
		//public bool HasRefKeyword { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentEntryNode;
	}
}