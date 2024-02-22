using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record BinaryOperatorNode : ISyntaxNode
{
    public BinaryOperatorNode(
        TypeClauseNode leftOperandTypeClauseNode,
        ISyntaxToken operatorToken,
        TypeClauseNode rightOperandTypeClauseNode,
        TypeClauseNode resultTypeClauseNode)
    {
        LeftOperandTypeClauseNode = leftOperandTypeClauseNode;
        OperatorToken = operatorToken;
        RightOperandTypeClauseNode = rightOperandTypeClauseNode;
        ResultTypeClauseNode = resultTypeClauseNode;

        ChildList = new ISyntax[]
        {
            OperatorToken
        }
        .ToImmutableArray();
    }

    public TypeClauseNode LeftOperandTypeClauseNode { get; }
    public ISyntaxToken OperatorToken { get; }
    public TypeClauseNode RightOperandTypeClauseNode { get; }
	public TypeClauseNode ResultTypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BinaryOperatorNode;
}