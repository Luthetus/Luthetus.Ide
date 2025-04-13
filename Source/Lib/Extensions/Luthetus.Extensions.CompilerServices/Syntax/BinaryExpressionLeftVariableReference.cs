using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax;

public sealed class BinaryExpressionLeftVariableReference : IExpressionNode
{
	public BinaryExpressionLeftVariableReference(
		VariableReference leftVariableReference,
		TypeReference leftOperandTypeReference,
		SyntaxToken operatorToken,
		TypeReference rightOperandTypeReference,
		TypeReference resultTypeReference,
		IExpressionNode rightExpressionNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.BinaryExpressionNode++;
		#endif
	
		LeftVariableReference = leftVariableReference;
		LeftOperandTypeReference = leftOperandTypeReference;
		OperatorToken = operatorToken;
		RightOperandTypeReference = rightOperandTypeReference;
		ResultTypeReference = resultTypeReference;
		RightExpressionNode = rightExpressionNode;
	}

	public BinaryExpressionLeftVariableReference(
			VariableReference leftVariableReference,
			TypeReference leftOperandTypeReference,
			SyntaxToken operatorToken,
			TypeReference rightOperandTypeReference,
			TypeReference resultTypeReference)
		: this(
			leftVariableReference,
			leftOperandTypeReference,
			operatorToken,
			rightOperandTypeReference,
			resultTypeReference,
			EmptyExpressionNode.Empty)
	{
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


