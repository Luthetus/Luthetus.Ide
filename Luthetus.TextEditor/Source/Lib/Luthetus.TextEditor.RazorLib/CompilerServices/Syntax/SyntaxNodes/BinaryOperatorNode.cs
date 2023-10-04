using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record BinaryOperatorNode : ISyntaxNode
{
    public BinaryOperatorNode(
        TypeClauseNode leftOperandTypeClauseNode,
        ISyntaxToken operatorToken,
        TypeClauseNode rightOperandTypeClauseNode,
        TypeClauseNode typeClauseNode)
    {
        LeftOperandTypeClauseNode = leftOperandTypeClauseNode;
        OperatorToken = operatorToken;
        RightOperandTypeClauseNode = rightOperandTypeClauseNode;
        TypeClauseNode = typeClauseNode;

        ChildBag = new ISyntax[]
        {
            OperatorToken
        }
        .ToImmutableArray();
    }

    public TypeClauseNode LeftOperandTypeClauseNode { get; }
    public ISyntaxToken OperatorToken { get; }
    public TypeClauseNode RightOperandTypeClauseNode { get; }
    public TypeClauseNode TypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BinaryOperatorNode;
}