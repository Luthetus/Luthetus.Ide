using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a function.
///
/// I'm going to experiment with making this a <see cref="IExpressionNode"/>.
/// Because the parameters are exclusive to the expression parsing logic,
/// and having to wrap this in a 'BadExpressionNode' when dealing with
/// expressions is very hard to read. (2024-10-26)
///
/// TODO: I don't like how this type has the 'OpenParenthesisToken, and CloseParenthesisToken'...
///       ...It was done this way in order to mirror the generic parameters.
///       |
/// 	  Because for generic parameters, the 'OpenAngleBracketToken, and CloseAngleBracketToken'
///       are tied to the existance of generic parameters
///       (i.e.: you must match at least 1 generic parameter if the 'OpenAngleBracketToken' is there.).
///       |
///       With the function parameters however, the 'OpenParenthesisToken' does not
///       mandate that at least 1 function parameter must be matched.
/// </summary>
public sealed class FunctionParametersListingNode : IExpressionNode
{
	public FunctionParametersListingNode(
		SyntaxToken openParenthesisToken,
		List<FunctionParameterEntryNode> functionParameterEntryNodeList,
		SyntaxToken closeParenthesisToken)
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.FunctionParametersListingNode++;
	
		OpenParenthesisToken = openParenthesisToken;
		FunctionParameterEntryNodeList = functionParameterEntryNodeList;
		CloseParenthesisToken = closeParenthesisToken;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken OpenParenthesisToken { get; }
	public List<FunctionParameterEntryNode> FunctionParameterEntryNodeList { get; }
	public SyntaxToken CloseParenthesisToken { get; private set; }
	TypeClauseNode IExpressionNode.ResultTypeClauseNode => TypeFacts.Pseudo.ToTypeClause();

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.FunctionParametersListingNode;

	public FunctionParametersListingNode SetCloseParenthesisToken(SyntaxToken closeParenthesisToken)
	{
		CloseParenthesisToken = closeParenthesisToken;

		_childListIsDirty = true;
		return this;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		// OpenParenthesisToken, FunctionParameterEntryNodeList.Length, CloseParenthesisToken,
		var childCount =
			1 +                                     // OpenParenthesisToken,
			FunctionParameterEntryNodeList.Count + // FunctionParameterEntryNodeList.Count,
			1;                                      // CloseParenthesisToken,

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		foreach (var item in FunctionParameterEntryNodeList)
		{
			childList[i++] = item;
		}
		childList[i++] = CloseParenthesisToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
