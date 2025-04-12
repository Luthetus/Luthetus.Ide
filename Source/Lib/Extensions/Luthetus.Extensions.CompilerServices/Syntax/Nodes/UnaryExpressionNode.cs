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

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public IExpressionNode Expression { get; }
	public UnaryOperatorNode UnaryOperatorNode { get; }
	public TypeReference ResultTypeReference => UnaryOperatorNode.ResultTypeReference;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.UnaryExpressionNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = new ISyntax[]
		{
			Expression,
			UnaryOperatorNode,
		};

		_childListIsDirty = false;
		return _childList;
	}
}