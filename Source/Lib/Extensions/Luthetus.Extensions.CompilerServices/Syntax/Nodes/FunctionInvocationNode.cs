using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class FunctionInvocationNode : IInvocationNode
{
	public FunctionInvocationNode(
		SyntaxToken functionInvocationIdentifierToken,
		FunctionDefinitionNode? functionDefinitionNode,
		GenericParametersListingNode? genericParametersListingNode,
		FunctionParameterListing functionParameterListing,
		TypeClauseNode resultTypeClauseNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.FunctionInvocationNode++;
		#endif
	
		FunctionInvocationIdentifierToken = functionInvocationIdentifierToken;
		FunctionDefinitionNode = functionDefinitionNode;
		GenericParametersListingNode = genericParametersListingNode;
		FunctionParameterListing = functionParameterListing;
		ResultTypeClauseNode = resultTypeClauseNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken FunctionInvocationIdentifierToken { get; }
	public FunctionDefinitionNode? FunctionDefinitionNode { get; }
	public GenericParametersListingNode? GenericParametersListingNode { get; private set; }
	public FunctionParameterListing FunctionParameterListing { get; private set; }
	public TypeClauseNode ResultTypeClauseNode { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.FunctionInvocationNode;
	
	public bool IsParsingFunctionParameters { get; set; }

	public FunctionInvocationNode SetGenericParametersListingNode(GenericParametersListingNode genericParametersListingNode)
	{
		GenericParametersListingNode = genericParametersListingNode;
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

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 3; // FunctionInvocationIdentifierToken, ...FunctionParametersListingNode, ResultTypeClauseNode,
		
		if (FunctionParameterListing.ConstructorWasInvoked)
		{
			childCount +=
				1 +                                                              // FunctionParametersListingNode.OpenParenthesisToken +
				FunctionParameterListing.FunctionParameterEntryList.Count + // FunctionParametersListingNode.FunctionParameterEntryList.Count
				1;                                                               // FunctionParametersListingNode.CloseParenthesisToken
		}
		
		if (GenericParametersListingNode is not null)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = FunctionInvocationIdentifierToken;
		if (FunctionDefinitionNode is not null)
			childList[i++] = FunctionDefinitionNode;
		if (GenericParametersListingNode is not null)
			childList[i++] = GenericParametersListingNode;
		
		if (FunctionParameterListing.ConstructorWasInvoked)
		{
			childList[i++] = FunctionParameterListing.OpenParenthesisToken;
			
			foreach (var item in FunctionParameterListing.FunctionParameterEntryList)
			{
				childList[i++] = item.ExpressionNode;
			}
			
			childList[i++] = FunctionParameterListing.CloseParenthesisToken;
		}
		
		childList[i++] = ResultTypeClauseNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}