using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record UnaryExpressionNode : IExpressionNode
{
    public UnaryExpressionNode(
        IExpressionNode expression,
        UnaryOperatorNode unaryOperatorNode)
    {
        Expression = expression;
        UnaryOperatorNode = unaryOperatorNode;

        ChildBag = new ISyntax[]
        {
            Expression,
            UnaryOperatorNode,
        }.ToImmutableArray();
    }

    public IExpressionNode Expression { get; }
    public UnaryOperatorNode UnaryOperatorNode { get; }
    public TypeClauseNode TypeClauseNode => UnaryOperatorNode.ResultTypeClauseNode;

    public ImmutableArray<ISyntax> ChildBag { get; init; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UnaryExpressionNode;
}