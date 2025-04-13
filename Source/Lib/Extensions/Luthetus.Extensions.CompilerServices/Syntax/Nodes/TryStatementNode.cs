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

	public TryStatementTryNode? TryNode { get; private set; }
	public TryStatementCatchNode? CatchNode { get; private set; }
	public TryStatementFinallyNode? FinallyNode { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.TryStatementNode;

	public void SetTryStatementTryNode(TryStatementTryNode tryStatementTryNode)
	{
		TryNode = tryStatementTryNode;
	}

	public void SetTryStatementCatchNode(TryStatementCatchNode tryStatementCatchNode)
	{
		CatchNode = tryStatementCatchNode;
	}

	public void SetTryStatementFinallyNode(TryStatementFinallyNode tryStatementFinallyNode)
	{
		FinallyNode = tryStatementFinallyNode;
	}
}
