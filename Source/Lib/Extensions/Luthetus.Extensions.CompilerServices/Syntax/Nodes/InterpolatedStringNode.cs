using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class InterpolatedStringNode : IExpressionNode
{
	/// <summary>
	/// The expression primary is set aside for a moment in order to parse
	/// the interpolated expressions.
	///
	/// Thus, pass it to the constructor so it can be restored as the expression primary
	/// after parsing the interpolated expressions.
	///
	/// (for example a BinaryExpressionNode might in reality be the true expression primary,
	///  but the InterpolatedStringNode is made expression primary for a time to parse its interpolated expressions first).
	///
	/// If 'toBeExpressionPrimary' is null then the 'InterpolatedStringNode' itself is the to be expression primary.
	/// </summary>
	public InterpolatedStringNode(
		SyntaxToken stringInterpolatedStartToken,
		SyntaxToken stringInterpolatedEndToken,
		IExpressionNode? toBeExpressionPrimary,
		TypeReference resultTypeReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.InterpolatedStringNode++;
		#endif
	
		StringInterpolatedStartToken = stringInterpolatedStartToken;
		StringInterpolatedEndToken = stringInterpolatedEndToken;
		ToBeExpressionPrimary = toBeExpressionPrimary;
		ResultTypeReference = resultTypeReference;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken StringInterpolatedStartToken { get; }
	public SyntaxToken StringInterpolatedEndToken { get; private set; }

	/// <summary>
	/// If 'ToBeExpressionPrimary' is null then the 'InterpolatedStringNode' itself is the to be expression primary.
	/// </summary>
	public IExpressionNode? ToBeExpressionPrimary { get; }

	public TypeReference ResultTypeReference { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.InterpolatedStringNode;

	public InterpolatedStringNode SetStringInterpolatedEndToken(SyntaxToken stringInterpolatedEndToken)
	{
		StringInterpolatedEndToken = stringInterpolatedEndToken;
		_childListIsDirty = true;
		return this;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 2; // StringInterpolatedStartToken, StringInterpolatedEndToken,

		if (ToBeExpressionPrimary is not null)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = StringInterpolatedStartToken;
		childList[i++] = StringInterpolatedEndToken;

		if (ToBeExpressionPrimary is not null)
			childList[i++] = ToBeExpressionPrimary;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
