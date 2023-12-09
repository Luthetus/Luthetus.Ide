using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record VariableDeclarationNodeTests
{
	[Fact]
	public void VariableDeclarationNode()
	{
		//public VariableDeclarationNode(
	 //       TypeClauseNode typeClauseNode,
	 //       IdentifierToken identifierToken,
	 //       VariableKind variableKind,
	 //       bool isInitialized)
	}

	[Fact]
	public void TypeClauseNode()
	{
		//public TypeClauseNode TypeClauseNode { get; }
	}

	[Fact]
	public void IdentifierToken()
	{
		//public IdentifierToken IdentifierToken { get; }
	}

	[Fact]
	public void VariableKind()
	{
		//public VariableKind VariableKind { get; }
	}

	[Fact]
	public void IsInitialized()
	{
		//public bool IsInitialized { get; }
	}

	[Fact]
	public void HasGetter()
	{
		//public bool HasGetter { get; set; }
	}

	[Fact]
	public void GetterIsAutoImplemented()
	{
		//public bool GetterIsAutoImplemented { get; set; }
	}

	[Fact]
	public void HasSetter()
	{
		//public bool HasSetter { get; set; }
	}

	[Fact]
	public void SetterIsAutoImplemented()
	{
		//public bool SetterIsAutoImplemented { get; set; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.VariableDeclarationStatementNode;
	}
}
