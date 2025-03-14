using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class AmbiguousIdentifierExpressionNode : IExpressionNode
{
	public AmbiguousIdentifierExpressionNode(
		SyntaxToken token,
		GenericParametersListingNode? genericParametersListingNode,
		TypeClauseNode resultTypeClauseNode)
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.AmbiguousIdentifierExpressionNode++;
	
		Token = token;
		GenericParametersListingNode = genericParametersListingNode;
		ResultTypeClauseNode = resultTypeClauseNode;
	}
	
	public int SuccessCount { get; set; }
	public int FailCount { get; set; }
	public bool _wasDecided = true;

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	
	/// <summary>Be wary that this is public. Something is being tested.</summary>
	public bool _childListIsDirty = true;

	public SyntaxToken Token { get; set; }
	public GenericParametersListingNode? GenericParametersListingNode { get; set; }
	public TypeClauseNode ResultTypeClauseNode { get; set; }
	public bool FollowsMemberAccessToken { get; set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.AmbiguousIdentifierExpressionNode;
	
	public void SetSharedInstance(
		SyntaxToken token,
		GenericParametersListingNode? genericParametersListingNode,
		TypeClauseNode resultTypeClauseNode,
		bool followsMemberAccessToken)
	{
		if (!_wasDecided)
		{
			++FailCount;
			// Console.WriteLine($"AmbiguousIdentifierExpressionNode !_wasDecided FailCount:{FailCount} SuccessCount:{SuccessCount}");
		}
		else
		{
			++SuccessCount;
		}
			
		_wasDecided = false;
		_childListIsDirty = true;
		
		Token = token;
		GenericParametersListingNode = genericParametersListingNode;
		ResultTypeClauseNode = resultTypeClauseNode;
		FollowsMemberAccessToken = followsMemberAccessToken;
	}

	public AmbiguousIdentifierExpressionNode SetGenericParametersListingNode(GenericParametersListingNode? genericParametersListingNode)
	{
		GenericParametersListingNode = genericParametersListingNode;

		_childListIsDirty = true;
		return this;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		// TODO: This method.
		_childList = Array.Empty<ISyntax>();

		_childListIsDirty = false;
		return _childList;
	}
}

