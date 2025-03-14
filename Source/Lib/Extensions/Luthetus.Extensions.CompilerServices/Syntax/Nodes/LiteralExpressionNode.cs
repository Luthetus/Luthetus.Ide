using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class LiteralExpressionNode : IExpressionNode
{
	public LiteralExpressionNode(SyntaxToken literalSyntaxToken, TypeClauseNode typeClauseNode)
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.LiteralExpressionNode++;
	
		LiteralSyntaxToken = literalSyntaxToken;
		ResultTypeClauseNode = typeClauseNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken LiteralSyntaxToken { get; }
	public TypeClauseNode ResultTypeClauseNode { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.LiteralExpressionNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 2; // LiteralSyntaxToken, ResultTypeClauseNode,

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = LiteralSyntaxToken;
		childList[i++] = ResultTypeClauseNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}
