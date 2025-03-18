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
		TypeClauseNode typeClauseNode,
		FunctionParameterListing functionParameterListing)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ConstructorInvocationExpressionNode++;
		#endif
	
		NewKeywordToken = newKeywordToken;
		ResultTypeClauseNode = typeClauseNode;
		FunctionParameterListing = functionParameterListing;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken NewKeywordToken { get; }
	public TypeClauseNode ResultTypeClauseNode { get; private set; }
	public FunctionParameterListing FunctionParameterListing { get; private set; }

	public ConstructorInvocationStageKind ConstructorInvocationStageKind { get; set; } = ConstructorInvocationStageKind.Unset;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ConstructorInvocationExpressionNode;
	
	public bool IsParsingFunctionParameters { get; set; }

	public ConstructorInvocationExpressionNode SetTypeClauseNode(TypeClauseNode? resultTypeClauseNode)
	{
		ResultTypeClauseNode = resultTypeClauseNode;

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

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 2; // NewKeywordToken, ResultTypeClauseNode
		
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
		childList[i++] = ResultTypeClauseNode;
		
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
