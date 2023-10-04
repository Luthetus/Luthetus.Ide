using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record LiteralExpressionNode : IExpressionNode
{
    public LiteralExpressionNode(ISyntaxToken literalSyntaxToken, TypeClauseNode? typeClauseNode)
    {
        LiteralSyntaxToken = literalSyntaxToken;
        TypeClauseNode = typeClauseNode;

        var children = new List<ISyntax>
        {
            LiteralSyntaxToken,
        };

        if (TypeClauseNode is not null)
            children.Add(TypeClauseNode);

        ChildBag = children.ToImmutableArray();
    }

    public ISyntaxToken LiteralSyntaxToken { get; }
    public TypeClauseNode? TypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.LiteralExpressionNode;
}
