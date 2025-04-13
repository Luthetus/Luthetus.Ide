using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax;

public sealed class BinaryExpressionLeftAndRightVariableReference : IExpressionNode
{
	public BinaryExpressionLeftAndRightVariableReference(
		VariableReference leftVariableReference,
		TypeReference leftOperandTypeReference,
		SyntaxToken operatorToken,
		TypeReference rightOperandTypeReference,
		TypeReference resultTypeReference,
		VariableReference rightVariableReference)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.BinaryExpressionNode++;
		#endif
	
		LeftVariableReference = leftVariableReference;
		LeftOperandTypeReference = leftOperandTypeReference;
		OperatorToken = operatorToken;
		RightOperandTypeReference = rightOperandTypeReference;
		ResultTypeReference = resultTypeReference;
		RightVariableReference = rightVariableReference;
	}

	public BinaryExpressionLeftAndRightVariableReference(
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
			VariableReference.Empty)
	{
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

