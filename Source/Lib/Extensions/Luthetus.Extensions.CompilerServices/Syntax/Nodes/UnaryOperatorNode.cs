namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class UnaryOperatorNode : ISyntaxNode
{
	public UnaryOperatorNode(
		TypeClauseNode operandTypeClauseNode,
		SyntaxToken operatorToken,
		TypeClauseNode resultTypeClauseNode)
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.UnaryOperatorNode++;
	
		OperandTypeClauseNode = operandTypeClauseNode;
		OperatorToken = operatorToken;
		ResultTypeClauseNode = resultTypeClauseNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public TypeClauseNode OperandTypeClauseNode { get; }
	public SyntaxToken OperatorToken { get; }
	public TypeClauseNode ResultTypeClauseNode { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.UnaryOperatorNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = new ISyntax[]
		{
			OperandTypeClauseNode,
			OperatorToken,
			ResultTypeClauseNode,
		};

		_childListIsDirty = false;
		return _childList;
	}
}