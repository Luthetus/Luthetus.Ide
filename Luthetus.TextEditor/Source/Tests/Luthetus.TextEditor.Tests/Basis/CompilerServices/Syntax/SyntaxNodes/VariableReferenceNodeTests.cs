using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record VariableReferenceNodeTests
{
	[Fact]
	public void VariableReferenceNode()
	{
		//public VariableReferenceNode(
	 //       IdentifierToken variableIdentifierToken,
	 //       VariableDeclarationNode variableDeclarationStatementNode)
	}

	[Fact]
	public void VariableIdentifierToken()
	{
		//public IdentifierToken VariableIdentifierToken { get; }
	}

	[Fact]
	public void VariableDeclarationStatementNode()
	{
		//public VariableDeclarationNode VariableDeclarationStatementNode { get; }
	}

	[Fact]
	public void TypeClauseNode()
	{
		//public TypeClauseNode TypeClauseNode => VariableDeclarationStatementNode.TypeClauseNode;
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
		//public SyntaxKind SyntaxKind => SyntaxKind.VariableReferenceNode;
	}
}
