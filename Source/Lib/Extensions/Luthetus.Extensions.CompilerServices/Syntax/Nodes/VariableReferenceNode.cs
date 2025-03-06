using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class VariableReferenceNode : IExpressionNode
{
	public VariableReferenceNode(
		SyntaxToken variableIdentifierToken,
		VariableDeclarationNode variableDeclarationNode)
	{
		VariableIdentifierToken = variableIdentifierToken;
		VariableDeclarationNode = variableDeclarationNode;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public SyntaxToken VariableIdentifierToken { get; }
	/// <summary>
	/// The <see cref="VariableDeclarationNode"/> is null when the variable is undeclared
	/// </summary>
	public VariableDeclarationNode VariableDeclarationNode { get; }
	public TypeClauseNode ResultTypeClauseNode => VariableDeclarationNode.TypeClauseNode;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.VariableReferenceNode;

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = new ISyntax[]
		{
			VariableIdentifierToken,
			VariableDeclarationNode,
		};

		_childListIsDirty = false;
		return _childList;
	}
}
