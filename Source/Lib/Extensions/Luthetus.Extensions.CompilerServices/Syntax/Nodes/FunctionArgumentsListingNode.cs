using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a function.
/// </summary>
public sealed class FunctionArgumentsListingNode : IExpressionNode
{
	public FunctionArgumentsListingNode(
		SyntaxToken openParenthesisToken,
		List<FunctionArgumentEntry> functionArgumentEntryList,
		SyntaxToken closeParenthesisToken)
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.FunctionArgumentsListingNode++;
	
		OpenParenthesisToken = openParenthesisToken;
		FunctionArgumentEntryList = functionArgumentEntryList;
		CloseParenthesisToken = closeParenthesisToken;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken OpenParenthesisToken { get; }
	public List<FunctionArgumentEntry> FunctionArgumentEntryList { get; }
	public SyntaxToken CloseParenthesisToken { get; }
	TypeClauseNode IExpressionNode.ResultTypeClauseNode => TypeFacts.Pseudo.ToTypeClause();

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentsListingNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		// OpenParenthesisToken, FunctionArgumentEntryList.Count, CloseParenthesisToken,
		var childCount =
			1 +                               // OpenParenthesisToken,
			FunctionArgumentEntryList.Count + // FunctionArgumentEntryList.Count,
			1;                                // CloseParenthesisToken,

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		foreach (var item in FunctionArgumentEntryList)
		{
			childList[i++] = item.VariableDeclarationNode;
		}
		childList[i++] = CloseParenthesisToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}