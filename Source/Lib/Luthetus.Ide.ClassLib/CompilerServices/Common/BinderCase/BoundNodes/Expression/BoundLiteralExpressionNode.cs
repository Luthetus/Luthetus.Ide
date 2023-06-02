using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;

public sealed record BoundLiteralExpressionNode : IBoundExpressionNode
{
    public BoundLiteralExpressionNode(
        ISyntaxToken literalSyntaxToken,
        Type resultType)
    {
        LiteralSyntaxToken = literalSyntaxToken;
        ResultType = resultType;

        Children = new ISyntax[]
        {
            literalSyntaxToken
        }.ToImmutableArray();
    }

    public ISyntaxToken LiteralSyntaxToken { get; }
    public Type ResultType { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundLiteralExpressionNode;
}
