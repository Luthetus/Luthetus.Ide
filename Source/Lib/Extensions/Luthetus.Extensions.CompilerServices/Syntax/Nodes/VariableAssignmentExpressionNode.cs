using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class VariableAssignmentExpressionNode : IExpressionNode
{
	public VariableAssignmentExpressionNode(
		SyntaxToken variableIdentifierToken,
		SyntaxToken equalsToken,
		IExpressionNode expressionNode)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.VariableAssignmentExpressionNode++;
		#endif
	
		VariableIdentifierToken = variableIdentifierToken;
		EqualsToken = equalsToken;
		ExpressionNode = expressionNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken VariableIdentifierToken { get; }
	public SyntaxToken EqualsToken { get; }
	public IExpressionNode ExpressionNode { get; private set; }
	public TypeReference ResultTypeReference => ExpressionNode.ResultTypeReference;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.VariableAssignmentExpressionNode;

	public VariableAssignmentExpressionNode SetExpressionNode(IExpressionNode expressionNode)
	{
		ExpressionNode = expressionNode;
		_childListIsDirty = true;
		return this;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = new ISyntax[]
		{
			VariableIdentifierToken,
			EqualsToken,
			ExpressionNode,
		};

		_childListIsDirty = false;
		return _childList;
	}
}
