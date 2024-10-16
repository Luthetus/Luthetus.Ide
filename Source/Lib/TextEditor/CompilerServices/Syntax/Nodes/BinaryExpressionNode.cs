using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class BinaryExpressionNode : IExpressionNode
{
    public BinaryExpressionNode(
        IExpressionNode leftExpressionNode,
        BinaryOperatorNode binaryOperatorNode,
        IExpressionNode rightExpressionNode)
    {
        LeftExpressionNode = leftExpressionNode;
        BinaryOperatorNode = binaryOperatorNode;
        RightExpressionNode = rightExpressionNode;

        ChildList = new ISyntax[]
        {
            LeftExpressionNode,
            BinaryOperatorNode,
            RightExpressionNode
        }.ToImmutableArray();
    }

    public IExpressionNode LeftExpressionNode { get; }
    public BinaryOperatorNode BinaryOperatorNode { get; }
    public IExpressionNode RightExpressionNode { get; }
    public TypeClauseNode ResultTypeClauseNode => BinaryOperatorNode.ResultTypeClauseNode;

    public ImmutableArray<ISyntax> ChildList { get; init; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BinaryExpressionNode;
}