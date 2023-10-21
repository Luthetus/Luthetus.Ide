using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record BinaryExpressionNode : IExpressionNode
{
    public BinaryExpressionNode(
        IExpressionNode leftExpressionNode,
        BinaryOperatorNode binaryOperatorNode,
        IExpressionNode rightExpressionNode)
    {
        LeftExpressionNode = leftExpressionNode;
        BinaryOperatorNode = binaryOperatorNode;
        RightExpressionNode = rightExpressionNode;

        ChildBag = new ISyntax[]
        {
            LeftExpressionNode,
            BinaryOperatorNode,
            RightExpressionNode
        }.ToImmutableArray();
    }

    public IExpressionNode LeftExpressionNode { get; }
    public BinaryOperatorNode BinaryOperatorNode { get; }
    public IExpressionNode RightExpressionNode { get; }
    public TypeClauseNode TypeClauseNode => BinaryOperatorNode.TypeClauseNode;

    public ImmutableArray<ISyntax> ChildBag { get; init; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BinaryExpressionNode;
}