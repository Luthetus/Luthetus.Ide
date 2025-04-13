using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax;

public sealed class BinaryExpressionLeftVariableReference : IExpressionNode
{
	public BinaryExpressionLeftVariableReference(BinaryExpressionNode binaryExpressionNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.BinaryExpressionLeftVariableReference++;
		#endif
	
		LeftVariableReference = new VariableReference((VariableReferenceNode)binaryExpressionNode.LeftExpressionNode);
		LeftOperandTypeReference = binaryExpressionNode.LeftOperandTypeReference;
		OperatorToken = binaryExpressionNode.OperatorToken;
		RightOperandTypeReference = binaryExpressionNode.RightOperandTypeReference;
		ResultTypeReference = binaryExpressionNode.ResultTypeReference;
		RightExpressionNode = binaryExpressionNode.RightExpressionNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public VariableReference LeftVariableReference { get; }
	public TypeReference LeftOperandTypeReference { get; }
	public SyntaxToken OperatorToken { get; }
	public TypeReference RightOperandTypeReference { get; }
	public TypeReference ResultTypeReference { get; }
	public IExpressionNode RightExpressionNode { get; private set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.BinaryExpressionLeftVariableReference;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 2; // OperatorToken, RightExpressionNode

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OperatorToken;
		childList[i++] = RightExpressionNode;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}

	public BinaryExpressionLeftVariableReference SetRightExpressionNode(IExpressionNode rightExpressionNode)
	{
		RightExpressionNode = rightExpressionNode;

		_childListIsDirty = true;
		return this;
	}
}


