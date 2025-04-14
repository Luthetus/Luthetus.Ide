using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax;

public sealed class BinaryExpressionLeftAndRightVariableReference : IExpressionNode
{
	public BinaryExpressionLeftAndRightVariableReference(BinaryExpressionNode binaryExpressionNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.BinaryExpressionLeftAndRightVariableReference++;
		#endif
		
		LeftVariableReference = new VariableReference((VariableReferenceNode)binaryExpressionNode.LeftExpressionNode);
		LeftOperandTypeReference = binaryExpressionNode.LeftOperandTypeReference;
		OperatorToken = binaryExpressionNode.OperatorToken;
		RightOperandTypeReference = binaryExpressionNode.RightOperandTypeReference;
		ResultTypeReference = binaryExpressionNode.ResultTypeReference;
		RightVariableReference = new VariableReference((VariableReferenceNode)binaryExpressionNode.RightExpressionNode);
		IsFabricated = binaryExpressionNode.IsFabricated;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public VariableReference LeftVariableReference { get; }
	public TypeReference LeftOperandTypeReference { get; }
	public SyntaxToken OperatorToken { get; }
	public TypeReference RightOperandTypeReference { get; }
	public TypeReference ResultTypeReference { get; }
	public VariableReference RightVariableReference { get; private set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.BinaryExpressionLeftAndRightVariableReference;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		var childCount = 3; // OperatorToken

		var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OperatorToken;

		_childList = childList;

		_childListIsDirty = false;
		return _childList;
	}

	public BinaryExpressionLeftAndRightVariableReference SetRightVariableReference(VariableReference rightVariableReference)
	{
		RightVariableReference = rightVariableReference;

		_childListIsDirty = true;
		return this;
	}
}

