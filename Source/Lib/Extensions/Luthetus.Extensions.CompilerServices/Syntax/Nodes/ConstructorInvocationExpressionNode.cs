using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ConstructorInvocationExpressionNode : IInvocationNode
{
	/// <summary>
	/// The <see cref="GenericParametersListingNode"/> is located
	/// on the <see cref="TypeClauseNode"/>.
	/// </summary>
	public ConstructorInvocationExpressionNode(
		SyntaxToken newKeywordToken,
		TypeReference typeReference,
		FunctionParameterListing functionParameterListing)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ConstructorInvocationExpressionNode++;
		#endif
	
		NewKeywordToken = newKeywordToken;
		ResultTypeReference = typeReference;
		FunctionParameterListing = functionParameterListing;
	}

	// private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken NewKeywordToken { get; }
	public TypeReference ResultTypeReference { get; private set; }
	public FunctionParameterListing FunctionParameterListing { get; private set; }

	public ConstructorInvocationStageKind ConstructorInvocationStageKind { get; set; } = ConstructorInvocationStageKind.Unset;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ConstructorInvocationExpressionNode;
	
	public bool IsParsingFunctionParameters { get; set; }

	public ConstructorInvocationExpressionNode SetTypeReference(TypeReference resultTypeReference)
	{
		ResultTypeReference = resultTypeReference;

		_childListIsDirty = true;
		return this;
	}
	
	public void SetFunctionParameterListing(FunctionParameterListing functionParameterListing)
	{
		FunctionParameterListing = functionParameterListing;
		_childListIsDirty = true;
	}
	
	public void SetFunctionParameterListingCloseParenthesisToken(SyntaxToken closeParenthesisToken)
	{
		FunctionParameterListing.SetCloseParenthesisToken(closeParenthesisToken);
		_childListIsDirty = true;
	}

	/*public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 1; // NewKeywordToken,
		
		if (FunctionParameterListing.ConstructorWasInvoked)
		{
			childCount +=
				1 +                                                              // FunctionParametersListingNode.OpenParenthesisToken +
				FunctionParameterListing.FunctionParameterEntryList.Count + // FunctionParametersListingNode.FunctionParameterEntryList.Count
				1;                                                               // FunctionParametersListingNode.CloseParenthesisToken
		}

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = NewKeywordToken;
		
		if (FunctionParameterListing.ConstructorWasInvoked)
		{
			childList[i++] = FunctionParameterListing.OpenParenthesisToken;
			
			foreach (var item in FunctionParameterListing.FunctionParameterEntryList)
			{
				childList[i++] = item.ExpressionNode;
			}
			
			childList[i++] = FunctionParameterListing.CloseParenthesisToken;
		}

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}*/
}
