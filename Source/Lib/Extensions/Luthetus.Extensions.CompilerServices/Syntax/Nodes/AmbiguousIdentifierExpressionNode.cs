using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class AmbiguousIdentifierExpressionNode : IGenericParameterNode
{
	public AmbiguousIdentifierExpressionNode(
		SyntaxToken token,
		GenericParameterListing genericParameterListing,
		TypeReference resultTypeReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.AmbiguousIdentifierExpressionNode++;
		#endif
	
		Token = token;
		GenericParameterListing = genericParameterListing;
		ResultTypeReference = resultTypeReference;
	}
	
	// public int SuccessCount { get; set; }
	// public int FailCount { get; set; }

	// private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	
	/// <summary>Be wary that this is public. Something is being tested.</summary>
	public bool _childListIsDirty = true;

	public SyntaxToken Token { get; set; }
	public GenericParameterListing GenericParameterListing { get; set; }
	public TypeReference ResultTypeReference { get; set; }
	public bool FollowsMemberAccessToken { get; set; }
	public bool HasQuestionMark { get; set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.AmbiguousIdentifierExpressionNode;
	
	public void SetSharedInstance(
		SyntaxToken token,
		GenericParameterListing genericParameterListing,
		TypeReference resultTypeReference,
		bool followsMemberAccessToken)
	{
		_childListIsDirty = true;
		
		Token = token;
		GenericParameterListing = genericParameterListing;
		ResultTypeReference = resultTypeReference;
		FollowsMemberAccessToken = followsMemberAccessToken;
		HasQuestionMark = false;
	}
	
	public bool IsParsingGenericParameters { get; set; }

	public void SetGenericParameterListing(GenericParameterListing genericParameterListing)
	{
		GenericParameterListing = genericParameterListing;
		_childListIsDirty = true;
	}
	
	public void SetGenericParameterListingCloseAngleBracketToken(SyntaxToken closeAngleBracketToken)
	{
		GenericParameterListing.SetCloseAngleBracketToken(closeAngleBracketToken);
		_childListIsDirty = true;
	}

	/*public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		// TODO: This method.
		_childList = Array.Empty<ISyntax>();

		_childListIsDirty = false;
		return _childList;
	}*/
}

