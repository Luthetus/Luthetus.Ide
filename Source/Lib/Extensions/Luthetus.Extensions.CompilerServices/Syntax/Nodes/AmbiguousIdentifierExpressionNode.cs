using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class AmbiguousIdentifierExpressionNode : IExpressionNode
{
	public AmbiguousIdentifierExpressionNode(
		SyntaxToken token,
		GenericParametersListingNode? genericParametersListingNode,
		TypeClauseNode resultTypeClauseNode)
	{
		// Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.AmbiguousIdentifierExpressionNode++;
	
		Token = token;
		GenericParametersListingNode = genericParametersListingNode;
		ResultTypeClauseNode = resultTypeClauseNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken Token { get; }
	public GenericParametersListingNode? GenericParametersListingNode { get; private set; }
	public TypeClauseNode ResultTypeClauseNode { get; }
	public bool FollowsMemberAccessToken { get; init; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.AmbiguousIdentifierExpressionNode;

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

