using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ParenthesizedExpressionNode : IExpressionNode
{
	public ParenthesizedExpressionNode(
		SyntaxToken openParenthesisToken,
		IExpressionNode innerExpression,
		SyntaxToken closeParenthesisToken)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ParenthesizedExpressionNode++;
		#endif
	
		OpenParenthesisToken = openParenthesisToken;
		InnerExpression = innerExpression;
		CloseParenthesisToken = closeParenthesisToken;
	}

	public ParenthesizedExpressionNode(SyntaxToken openParenthesisToken, TypeReference typeReference)
		: this(openParenthesisToken, EmptyExpressionNode.Empty, default)
	{
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken OpenParenthesisToken { get; }
	public IExpressionNode InnerExpression { get; private set; }
	public SyntaxToken CloseParenthesisToken { get; private set; }
	public TypeReference ResultTypeReference => InnerExpression.ResultTypeReference;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ParenthesizedExpressionNode;

	public ParenthesizedExpressionNode SetCloseParenthesisToken(SyntaxToken closeParenthesisToken)
	{
		CloseParenthesisToken = closeParenthesisToken;

		_childListIsDirty = true;
		return this;
	}

	public ParenthesizedExpressionNode SetInnerExpression(IExpressionNode innerExpression)
	{
		InnerExpression = innerExpression;

		_childListIsDirty = true;
		return this;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 3; // OpenParenthesisToken, InnerExpression, CloseParenthesisToken,

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		childList[i++] = InnerExpression;
		childList[i++] = CloseParenthesisToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
