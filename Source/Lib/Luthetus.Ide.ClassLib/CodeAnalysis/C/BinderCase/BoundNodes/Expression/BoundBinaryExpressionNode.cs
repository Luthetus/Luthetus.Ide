using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.BinderCase.BoundNodes.Expression;

public class BoundBinaryExpressionNode : IBoundExpressionNode
{
    public BoundBinaryExpressionNode(
        IBoundExpressionNode leftBoundExpressionNode,
        BoundBinaryOperatorNode boundBinaryOperatorNode,
        IBoundExpressionNode rightBoundExpressionNode)
    {
        LeftBoundExpressionNode = leftBoundExpressionNode;
        BoundBinaryOperatorNode = boundBinaryOperatorNode;
        RightBoundExpressionNode = rightBoundExpressionNode;

        Children = new ISyntax[]
        {
            LeftBoundExpressionNode,
            BoundBinaryOperatorNode,
            RightBoundExpressionNode
        }.ToImmutableArray();
    }

    public IBoundExpressionNode LeftBoundExpressionNode { get; }
    public BoundBinaryOperatorNode BoundBinaryOperatorNode { get; }
    public IBoundExpressionNode RightBoundExpressionNode { get; }
    public Type ResultType => BoundBinaryOperatorNode.ResultType;

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundBinaryExpressionNode;
}
