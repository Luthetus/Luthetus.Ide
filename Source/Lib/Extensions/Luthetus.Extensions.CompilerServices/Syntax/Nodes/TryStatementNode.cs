using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class TryStatementNode : ISyntaxNode
{
	public TryStatementNode(
		TryStatementTryNode? tryNode,
		TryStatementCatchNode? catchNode,
		TryStatementFinallyNode? finallyNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TryStatementNode++;
		#endif
	
		TryNode = tryNode;
		CatchNode = catchNode;
		FinallyNode = finallyNode;
	}

	// private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public TryStatementTryNode? TryNode { get; private set; }
	public TryStatementCatchNode? CatchNode { get; private set; }
	public TryStatementFinallyNode? FinallyNode { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.TryStatementNode;

	/*public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 0;
		if (TryNode is not null)
			childCount++;
		if (CatchNode is not null)
			childCount++;
		if (FinallyNode is not null)
			childCount++;

		var childList = new ISyntax[childCount];
		var i = 0;

		if (TryNode is not null)
			childList[i++] = TryNode;
		if (CatchNode is not null)
			childList[i++] = CatchNode;
		if (FinallyNode is not null)
			childList[i++] = FinallyNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}*/

	public void SetTryStatementTryNode(TryStatementTryNode tryStatementTryNode)
	{
		TryNode = tryStatementTryNode;
		_childListIsDirty = true;
	}

	public void SetTryStatementCatchNode(TryStatementCatchNode tryStatementCatchNode)
	{
		CatchNode = tryStatementCatchNode;
		_childListIsDirty = true;
	}

	public void SetTryStatementFinallyNode(TryStatementFinallyNode tryStatementFinallyNode)
	{
		FinallyNode = tryStatementFinallyNode;
		_childListIsDirty = true;
	}
}
