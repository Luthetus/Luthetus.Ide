using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ExplicitCastNode : IExpressionNode
{
	public ExplicitCastNode(
		SyntaxToken openParenthesisToken,
		TypeReference resultTypeReference,
		SyntaxToken closeParenthesisToken)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ExplicitCastNode++;
		#endif
	
		OpenParenthesisToken = openParenthesisToken;
		ResultTypeReference = resultTypeReference;
		CloseParenthesisToken = closeParenthesisToken;
	}

	// private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public ExplicitCastNode(SyntaxToken openParenthesisToken, TypeReference resultTypeReference)
		: this(openParenthesisToken, resultTypeReference, default)
	{
	}

	public SyntaxToken OpenParenthesisToken { get; }
	public TypeReference ResultTypeReference { get; }
	public SyntaxToken CloseParenthesisToken { get; private set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ExplicitCastNode;

	public ExplicitCastNode SetCloseParenthesisToken(SyntaxToken closeParenthesisToken)
	{
		CloseParenthesisToken = closeParenthesisToken;

		_childListIsDirty = true;
		return this;
	}

	/*public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 2; // OpenParenthesisToken, ResultTypeReference, CloseParenthesisToken,

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		childList[i++] = CloseParenthesisToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}*/
}
