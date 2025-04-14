using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class UnaryExpressionNode : IExpressionNode
{
	public UnaryExpressionNode(
		IExpressionNode expression,
		UnaryOperatorNode unaryOperatorNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.UnaryExpressionNode++;
		#endif
	
		Expression = expression;
		UnaryOperatorNode = unaryOperatorNode;
	}

	public IExpressionNode Expression { get; }
	public UnaryOperatorNode UnaryOperatorNode { get; }
	public TypeReference ResultTypeReference => UnaryOperatorNode.ResultTypeReference;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.UnaryExpressionNode;
}