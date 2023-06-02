using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;

public sealed record BoundTypeNode : ISyntaxNode
{
    public BoundTypeNode(
        Type type,
        ISyntaxToken token)
    {
        Type = type;
        Token = token;

        Children = new ISyntax[]
        {
            token
        }.ToImmutableArray();
    }

    public Type Type { get; init; }
    public ISyntaxToken Token { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundTypeNode;
}
