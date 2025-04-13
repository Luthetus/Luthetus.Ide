using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax;

public sealed class BinaryExpressionRightVariableReference : IExpressionNode
{
	public BinaryExpressionRightVariableReference(BinaryExpressionNode binaryExpressionNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.BinaryExpressionRightVariableReference++;
		#endif
		
		LeftExpressionNode = binaryExpressionNode.LeftExpressionNode;
		LeftOperandTypeReference = binaryExpressionNode.LeftOperandTypeReference;
		OperatorToken = binaryExpressionNode.OperatorToken;
		RightOperandTypeReference = binaryExpressionNode.RightOperandTypeReference;
		ResultTypeReference = binaryExpressionNode.ResultTypeReference;
		RightVariableReference = new VariableReference((VariableReferenceNode)binaryExpressionNode.RightExpressionNode);
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public IExpressionNode LeftExpressionNode { get; private set; }
	public TypeReference LeftOperandTypeReference { get; }
	public SyntaxToken OperatorToken { get; }
	public TypeReference RightOperandTypeReference { get; }
	public TypeReference ResultTypeReference { get; }
	public VariableReference RightVariableReference { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.BinaryExpressionRightVariableReference;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 3; // LeftExpressionNode, OperatorToken

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = LeftExpressionNode;
		childList[i++] = OperatorToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}

	public BinaryExpressionRightVariableReference SetLeftExpressionNode(IExpressionNode expressionNode)
	{
		LeftExpressionNode = expressionNode;

		_childListIsDirty = true;
		return this;
	}
}

