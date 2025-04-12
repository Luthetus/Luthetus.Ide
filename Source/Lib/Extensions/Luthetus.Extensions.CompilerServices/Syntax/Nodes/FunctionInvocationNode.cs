using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class FunctionInvocationNode : IInvocationNode, IGenericParameterNode
{
	public FunctionInvocationNode(
		SyntaxToken functionInvocationIdentifierToken,
		FunctionDefinitionNode? functionDefinitionNode,
		GenericParameterListing genericParameterListing,
		FunctionParameterListing functionParameterListing,
		TypeReference resultTypeReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.FunctionInvocationNode++;
		#endif
	
		FunctionInvocationIdentifierToken = functionInvocationIdentifierToken;
		FunctionDefinitionNode = functionDefinitionNode;
		GenericParameterListing = genericParameterListing;
		FunctionParameterListing = functionParameterListing;
		ResultTypeReference = resultTypeReference;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken FunctionInvocationIdentifierToken { get; }
	public FunctionDefinitionNode? FunctionDefinitionNode { get; }
	public GenericParameterListing GenericParameterListing { get; set; }
	public FunctionParameterListing FunctionParameterListing { get; private set; }
	public TypeReference ResultTypeReference { get; private set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.FunctionInvocationNode;
	
	public bool IsParsingFunctionParameters { get; set; }
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
	
	/// <summary>
	/// TODO: 'BindFunctionInvocationNode' takes the instance of the 'FunctionInvocationNode',
	/// but in order to know the result type clause node we need to invoke
	/// 'BindFunctionInvocationNode', this is odd?
	/// </summary>
	public void SetResultTypeReference(TypeReference typeReference)
	{
		ResultTypeReference = typeReference;
		_childListIsDirty = true;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 2; // FunctionInvocationIdentifierToken, ...FunctionParametersListingNode,
		
		if (FunctionParameterListing.ConstructorWasInvoked)
		{
			childCount +=
				1 +                                                              // FunctionParametersListingNode.OpenParenthesisToken +
				FunctionParameterListing.FunctionParameterEntryList.Count + // FunctionParametersListingNode.FunctionParameterEntryList.Count
				1;                                                               // FunctionParametersListingNode.CloseParenthesisToken
		}
		
		if (GenericParameterListing.ConstructorWasInvoked)
		{
			childCount +=
				1 +                                                       // GenericParameterListing.OpenAngleBracketToken
				GenericParameterListing.GenericParameterEntryList.Count + // GenericParameterListing.GenericParameterEntryList.Count
				1;                                                        // GenericParameterListing.CloseAngleBracketToken
		}

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = FunctionInvocationIdentifierToken;
		if (FunctionDefinitionNode is not null)
			childList[i++] = FunctionDefinitionNode;
		if (GenericParameterListing.ConstructorWasInvoked)
		{
			childList[i++] = GenericParameterListing.OpenAngleBracketToken;
			foreach (var entry in GenericParameterListing.GenericParameterEntryList)
			{
				childList[i++] = entry.TypeClauseNode;
			}
			childList[i++] = GenericParameterListing.CloseAngleBracketToken;
		}
		
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
	}
}