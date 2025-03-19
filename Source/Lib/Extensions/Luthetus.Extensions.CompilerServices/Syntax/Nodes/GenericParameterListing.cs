using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a syntax which contains a generic type.
/// </summary>
public struct GenericParameterListing
{
	public GenericParameterListing(
		SyntaxToken openAngleBracketToken,
		List<GenericParameterEntry> genericParameterEntryList,
		SyntaxToken closeAngleBracketToken)
	{
		OpenAngleBracketToken = openAngleBracketToken;
		GenericParameterEntryList = genericParameterEntryList;
		CloseAngleBracketToken = closeAngleBracketToken;
	}

	public SyntaxToken OpenAngleBracketToken { get; }
	public List<GenericParameterEntry> GenericParameterEntryList { get; }
	public SyntaxToken CloseAngleBracketToken { get; private set; }
	
	public bool ConstructorWasInvoked => OpenAngleBracketToken.ConstructorWasInvoked;

	public void SetCloseAngleBracketToken(SyntaxToken closeAngleBracketToken)
	{
		CloseAngleBracketToken = closeAngleBracketToken;
	}
}