using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// I'm not sure how to go about property initialization of an object versus collection initialization.
/// For now I'll rather hackily set the <see cref="ExpressionNode"/> only if a comma immediately follows it.
///
/// Then to know if an instance of this type represents collection initialization
/// one can check <see cref="IsCollectionInitialization"/> to know whether
/// the token was 'default' or not.
/// </summary>
public sealed class ObjectInitializationParameterEntryNode : ISyntaxNode
{
	public ObjectInitializationParameterEntryNode(
		SyntaxToken propertyIdentifierToken,
		SyntaxToken equalsToken,
		IExpressionNode expressionNode)
	{
		PropertyIdentifierToken = propertyIdentifierToken;
		EqualsToken = equalsToken;
		ExpressionNode = expressionNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken PropertyIdentifierToken { get; set; }
	public SyntaxToken EqualsToken { get; set; }
	public IExpressionNode ExpressionNode { get; set; }
	public bool IsCollectionInitialization => !PropertyIdentifierToken.ConstructorWasInvoked;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationParameterEntryNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 2; // PropertyIdentifierToken, ExpressionNode,

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = PropertyIdentifierToken;
		childList[i++] = ExpressionNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}