namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>TODO: Correctly implement this node. For now, just skip over it when parsing.</summary>
[Obsolete($"Use: {nameof(ConstructorInvocationExpressionNode)}.{nameof(ConstructorInvocationExpressionNode.ObjectInitializationParametersListingNode)}")]
public sealed class ObjectInitializationNode : ISyntaxNode
{
	public ObjectInitializationNode(SyntaxToken openBraceToken, SyntaxToken closeBraceToken)
	{
		// Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ObjectInitializationNode++;
	
		OpenBraceToken = openBraceToken;
		CloseBraceToken = closeBraceToken;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken OpenBraceToken { get; }
	public SyntaxToken CloseBraceToken { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 2; // OpenBraceToken, CloseBraceToken,

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenBraceToken;
		childList[i++] = CloseBraceToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}