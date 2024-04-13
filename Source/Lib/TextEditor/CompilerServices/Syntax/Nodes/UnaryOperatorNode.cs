using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record UnaryOperatorNode : ISyntaxNode
{
    public UnaryOperatorNode(
        TypeClauseNode operandTypeClauseNode,
        ISyntaxToken operatorToken,
        TypeClauseNode resultTypeClauseNode)
    {
        OperandTypeClauseNode = operandTypeClauseNode;
        OperatorToken = operatorToken;
        ResultTypeClauseNode = resultTypeClauseNode;

        ChildList = new ISyntax[]
        {
            OperandTypeClauseNode,
            OperatorToken,
            ResultTypeClauseNode,
        }
        .ToImmutableArray();
    }

    public TypeClauseNode OperandTypeClauseNode { get; }
    public ISyntaxToken OperatorToken { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UnaryOperatorNode;
}