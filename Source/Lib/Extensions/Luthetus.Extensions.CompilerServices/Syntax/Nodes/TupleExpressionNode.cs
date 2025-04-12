using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class TupleExpressionNode : IExpressionNode
{
	public TupleExpressionNode()
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.TupleExpressionNode++;
		#endif
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public TypeReference ResultTypeReference { get; } = TypeFacts.Empty.ToTypeClause();

	public List<IExpressionNode> InnerExpressionList { get; } = new();

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.TupleExpressionNode;

	public void AddInnerExpressionNode(IExpressionNode expressionNode)
	{
		InnerExpressionList.Add(expressionNode);
		_childListIsDirty = true;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = InnerExpressionList.ToArray();

		_childListIsDirty = false;
		return _childList;
	}
}
