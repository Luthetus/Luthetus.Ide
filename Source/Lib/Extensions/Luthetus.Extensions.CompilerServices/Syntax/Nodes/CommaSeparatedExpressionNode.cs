using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// <see cref="CommaSeparatedExpressionNode"/>, <see cref="TupleExpressionNode"/>,
/// and <see cref="LambdaExpressionNode"/> are all related.
///
/// But, <see cref="CommaSeparatedExpressionNode"/> is an incomplete expression.
/// The <see cref="CommaSeparatedExpressionNode"/> goes on to become either a
/// <see cref="TupleExpressionNode"/>, or <see cref="LambdaExpressionNode"/>
/// (by checking the tokens that follow the close parenthesis token).
/// </summary>
[Obsolete($"Use '{nameof(AmbiguousParenthesizedExpressionNode)}'")]
public sealed class CommaSeparatedExpressionNode : IExpressionNode
{
	public CommaSeparatedExpressionNode()
	{
		// Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.CommaSeparatedExpressionNode++;
	}

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

	public TypeClauseNode ResultTypeClauseNode { get; } = TypeFacts.Pseudo.ToTypeClause();

	public List<IExpressionNode> InnerExpressionList { get; } = new();
	public SyntaxToken CloseParenthesisToken { get; set; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.CommaSeparatedExpressionNode;

	public void AddInnerExpressionNode(IExpressionNode expressionNode)
	{
		InnerExpressionList.Add(expressionNode);
		_childListIsDirty = true;
	}

	public IReadOnlyList<ISyntax> GetChildList()
	{
		if (!_childListIsDirty)
			return _childList;

		_childList = InnerExpressionList.ToArray();

		_childListIsDirty = false;
		return _childList;
	}
}
