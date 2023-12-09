using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// One usage of the <see cref="IdempotentExpressionNode"/> is for a <see cref="ParenthesizedExpressionNode"/>
/// which has no <see cref="ParenthesizedExpressionNode.InnerExpression"/>
/// </summary>
public sealed record IdempotentExpressionNodeTests
{
    public IdempotentExpressionNode(TypeClauseNode typeClauseNode)
    {
        TypeClauseNode = typeClauseNode;

        var children = new List<ISyntax>
        {
            TypeClauseNode
        };

        ChildBag = children.ToImmutableArray();
    }

    public TypeClauseNode TypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.IdempotentExpressionNode;
}