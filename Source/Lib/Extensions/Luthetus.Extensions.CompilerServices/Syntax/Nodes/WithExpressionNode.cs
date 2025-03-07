using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class WithExpressionNode : IExpressionNode
{
	public WithExpressionNode(VariableReferenceNode variableReferenceNode)
	{
		VariableReferenceNode = variableReferenceNode;
		ResultTypeClauseNode = variableReferenceNode.ResultTypeClauseNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public VariableReferenceNode VariableReferenceNode { get; }
	public TypeClauseNode ResultTypeClauseNode { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.WithExpressionNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = new ISyntax[]
		{
			VariableReferenceNode,
			ResultTypeClauseNode,
		};

		_childListIsDirty = false;
		return _childList;
	}
}
