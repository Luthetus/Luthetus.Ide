using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a function.
/// </summary>
public struct FunctionArgumentListing
{
	public FunctionArgumentListing(
		SyntaxToken openParenthesisToken,
		List<FunctionArgumentEntry> functionArgumentEntryList,
		SyntaxToken closeParenthesisToken)
	{
		OpenParenthesisToken = openParenthesisToken;
		FunctionArgumentEntryList = functionArgumentEntryList;
		CloseParenthesisToken = closeParenthesisToken;
	}

	public SyntaxToken OpenParenthesisToken { get; }
	public List<FunctionArgumentEntry> FunctionArgumentEntryList { get; }
	public SyntaxToken CloseParenthesisToken { get; private set; }
	
	public bool ConstructorWasInvoked => OpenParenthesisToken.ConstructorWasInvoked;
	
	public void SetCloseParenthesisToken(SyntaxToken closeParenthesisToken)
	{
		CloseParenthesisToken = closeParenthesisToken;
	}
}
