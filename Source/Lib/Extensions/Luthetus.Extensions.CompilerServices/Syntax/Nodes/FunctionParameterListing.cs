using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a function.
/// </summary>
public struct FunctionParameterListing
{
	public FunctionParameterListing(
		SyntaxToken openParenthesisToken,
		List<FunctionParameterEntry> functionParameterEntryList,
		SyntaxToken closeParenthesisToken)
	{
		OpenParenthesisToken = openParenthesisToken;
		FunctionParameterEntryList = functionParameterEntryList;
		CloseParenthesisToken = closeParenthesisToken;
	}

	public SyntaxToken OpenParenthesisToken { get; }
	public List<FunctionParameterEntry> FunctionParameterEntryList { get; }
	public SyntaxToken CloseParenthesisToken { get; set; }
	
	public bool ConstructorWasInvoked => OpenParenthesisToken.ConstructorWasInvoked;
	
	public void SetCloseParenthesisToken(SyntaxToken closeParenthesisToken)
	{
		CloseParenthesisToken = closeParenthesisToken;
	}
}
