using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Expression;

public sealed record LiteralExpressionNode : IExpressionNode
{
    public LiteralExpressionNode(ISyntaxToken literalSyntaxToken)
    {
        LiteralSyntaxToken = literalSyntaxToken;

        Children = new ISyntax[]
        {
            LiteralSyntaxToken
        }.ToImmutableArray();
    }

    public ISyntaxToken LiteralSyntaxToken { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.LiteralExpressionNode;
}
