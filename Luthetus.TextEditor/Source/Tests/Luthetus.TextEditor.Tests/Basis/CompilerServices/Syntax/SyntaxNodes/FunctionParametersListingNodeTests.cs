using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record FunctionParametersListingNodeTests
{
	[Fact]
	public void FunctionParametersListingNode()
	{
		//public FunctionParametersListingNode(
		//       OpenParenthesisToken openParenthesisToken,
		//       ImmutableArray<FunctionParameterEntryNode> functionParameterEntryNodes,
		//       CloseParenthesisToken closeParenthesisToken)
	}

	[Fact]
	public void OpenParenthesisToken()
	{
		//public OpenParenthesisToken OpenParenthesisToken { get; }
	}

	[Fact]
	public void FunctionParameterEntryNodeBag()
	{
		//public ImmutableArray<FunctionParameterEntryNode> FunctionParameterEntryNodeBag { get; }
	}

	[Fact]
	public void CloseParenthesisToken()
	{
		//public CloseParenthesisToken CloseParenthesisToken { get; }
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
		//public SyntaxKind SyntaxKind => SyntaxKind.FunctionParametersListingNode;
	}
}