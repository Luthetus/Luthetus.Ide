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

	public TryStatementTryNode? TryNode { get; set; }
	public TryStatementCatchNode? CatchNode { get; set; }
	public TryStatementFinallyNode? FinallyNode { get; set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.TryStatementNode;
}
