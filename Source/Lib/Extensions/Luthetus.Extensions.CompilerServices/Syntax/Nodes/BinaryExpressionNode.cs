using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class BinaryExpressionNode : IExpressionNode
{
	public BinaryExpressionNode(
		IExpressionNode leftExpressionNode,
		TypeClauseNode leftOperandTypeClauseNode,
		SyntaxToken operatorToken,
		TypeClauseNode rightOperandTypeClauseNode,
		TypeClauseNode resultTypeClauseNode,
		IExpressionNode rightExpressionNode)
	{
		LeftExpressionNode = leftExpressionNode;
		LeftOperandTypeClauseNode = leftOperandTypeClauseNode;
		OperatorToken = operatorToken;
		RightOperandTypeClauseNode = rightOperandTypeClauseNode;
		ResultTypeClauseNode = resultTypeClauseNode;
		RightExpressionNode = rightExpressionNode;
	}

	public BinaryExpressionNode(
			IExpressionNode leftExpressionNode,
			TypeClauseNode leftOperandTypeClauseNode,
			SyntaxToken operatorToken,
			TypeClauseNode rightOperandTypeClauseNode,
			TypeClauseNode resultTypeClauseNode)
		: this(
			leftExpressionNode,
			leftOperandTypeClauseNode,
			operatorToken,
			rightOperandTypeClauseNode,
			resultTypeClauseNode,
			new EmptyExpressionNode(rightOperandTypeClauseNode))
	{
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public IExpressionNode LeftExpressionNode { get; }
	
	/// <summary>
	/// There are 76,366 instances of 'BinaryOperatorNode' that are created
	/// by the solution wide parse on 'Luthetus.Ide.sln.
	/// For this reason, the type will attempt to be inlined on the 'BinaryExpressionNode'.
	/// Because, given how things currently are written, this seems fine.
	/// </summary>
	public TypeClauseNode LeftOperandTypeClauseNode { get; }
	public SyntaxToken OperatorToken { get; }
	public TypeClauseNode RightOperandTypeClauseNode { get; }
	public TypeClauseNode ResultTypeClauseNode { get; }
	
	public IExpressionNode RightExpressionNode { get; private set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.BinaryExpressionNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 3; // LeftExpressionNode, OperatorToken, RightExpressionNode

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = LeftExpressionNode;
		childList[i++] = OperatorToken;
		childList[i++] = RightExpressionNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}

	public BinaryExpressionNode SetRightExpressionNode(IExpressionNode rightExpressionNode)
	{
		RightExpressionNode = rightExpressionNode;

		_childListIsDirty = true;
		return this;
	}
}