using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax;

public sealed class BinaryExpressionRightVariableReference : IExpressionNode
{
	public BinaryExpressionRightVariableReference(
		IExpressionNode leftExpressionNode,
		TypeReference leftOperandTypeReference,
		SyntaxToken operatorToken,
		TypeReference rightOperandTypeReference,
		TypeReference resultTypeReference,
		VariableReference rightVariableReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.BinaryExpressionNode++;
		#endif
	
		LeftExpressionNode = leftExpressionNode;
		LeftOperandTypeReference = leftOperandTypeReference;
		OperatorToken = operatorToken;
		RightOperandTypeReference = rightOperandTypeReference;
		ResultTypeReference = resultTypeReference;
		RightVariableReference = rightVariableReference;
	}

	public BinaryExpressionRightVariableReference(
			IExpressionNode leftExpressionNode,
			TypeReference leftOperandTypeReference,
			SyntaxToken operatorToken,
			TypeReference rightOperandTypeReference,
			TypeReference resultTypeReference)
		: this(
			leftExpressionNode,
			leftOperandTypeReference,
			operatorToken,
			rightOperandTypeReference,
			resultTypeReference,
			VariableReference.Empty)
	{
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public IExpressionNode LeftExpressionNode { get; }
	public TypeReference LeftOperandTypeReference { get; }
	public SyntaxToken OperatorToken { get; }
	public TypeReference RightOperandTypeReference { get; }
	public TypeReference ResultTypeReference { get; }
	public VariableReference RightVariableReference { get; private set; }

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

	public BinaryExpressionRightVariableReference SetRightVariableReference(VariableReference rightVariableReference)
	{
		RightVariableReference = rightVariableReference;

		_childListIsDirty = true;
		return this;
	}
}

