using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="UnaryOperatorNode"/>
/// </summary>
public sealed record UnaryOperatorNodeTests
{
    /// <summary>
    /// <see cref="UnaryOperatorNode(TypeClauseNode, ISyntaxToken, TypeClauseNode)"/>
	/// <br/>----<br/>
	/// <see cref="UnaryOperatorNode.OperandTypeClauseNode"/>
	/// <see cref="UnaryOperatorNode.OperatorToken"/>
	/// <see cref="UnaryOperatorNode.ResultTypeClauseNode"/>
	/// <see cref="UnaryOperatorNode.ChildList"/>
	/// <see cref="UnaryOperatorNode.IsFabricated"/>
	/// <see cref="UnaryOperatorNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		throw new NotImplementedException();
	}
}