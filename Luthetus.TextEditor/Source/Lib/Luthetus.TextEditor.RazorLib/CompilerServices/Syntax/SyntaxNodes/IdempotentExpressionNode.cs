using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// One usage of the <see cref="IdempotentExpressionNode"/> is for a <see cref="ParenthesizedExpressionNode"/>
/// which has no <see cref="ParenthesizedExpressionNode.InnerExpression"/>
/// </summary>
public sealed record IdempotentExpressionNode : IExpressionNode
{
    public IdempotentExpressionNode(TypeClauseNode typeClauseNode)
    {
        ResultTypeClauseNode = typeClauseNode;

        var children = new List<ISyntax>
        {
            ResultTypeClauseNode
        };

        ChildList = children.ToImmutableArray();
    }

    public TypeClauseNode ResultTypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.IdempotentExpressionNode;
}