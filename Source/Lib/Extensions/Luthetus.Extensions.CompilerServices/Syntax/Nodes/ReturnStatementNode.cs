using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class ReturnStatementNode : IExpressionNode
{
	public ReturnStatementNode(SyntaxToken keywordToken, IExpressionNode expressionNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.ReturnStatementNode++;
		#endif
	
		KeywordToken = keywordToken;
		ExpressionNode = expressionNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken KeywordToken { get; }
	public IExpressionNode ExpressionNode { get; set; }
	public TypeReference ResultTypeReference => ExpressionNode.ResultTypeReference;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.ReturnStatementNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 2; // KeywordToken, ExpressionNode

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = KeywordToken;
		childList[i++] = ExpressionNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}
}