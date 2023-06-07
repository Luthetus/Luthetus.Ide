using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;

public sealed record BoundBinaryExpressionNode : IBoundExpressionNode
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

    public IBoundExpressionNode LeftBoundExpressionNode { get; init; }
    public BoundBinaryOperatorNode BoundBinaryOperatorNode { get; init; }
    public IBoundExpressionNode RightBoundExpressionNode { get; init; }
    public Type ResultType => BoundBinaryOperatorNode.ResultType;
    public BoundClassReferenceNode? BoundClassReferenceNode { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundBinaryExpressionNode;
}
