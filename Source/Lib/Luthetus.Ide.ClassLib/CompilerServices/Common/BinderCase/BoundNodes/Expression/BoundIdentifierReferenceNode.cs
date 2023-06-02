using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;

public sealed record BoundIdentifierReferenceNode : IBoundExpressionNode
{
    public BoundIdentifierReferenceNode(
        IdentifierToken identifierToken,
        Type resultType)
    {
        IdentifierToken = identifierToken;
        ResultType = resultType;

        Children = new ISyntax[]
        {
            identifierToken
        }.ToImmutableArray();
    }

    public IdentifierToken IdentifierToken { get; }
    public Type ResultType { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundIdentifierReferenceNode;
}
