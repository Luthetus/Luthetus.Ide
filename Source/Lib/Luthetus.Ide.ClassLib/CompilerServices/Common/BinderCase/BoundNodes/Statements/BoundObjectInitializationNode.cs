using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

/// <summary>TODO: Correctly implement this node. For now, just skip over it when parsing.</summary>
public sealed record BoundObjectInitializationNode : ISyntaxNode
{
    public BoundObjectInitializationNode(
        OpenBraceToken openBraceToken,
        CloseBraceToken closeBraceToken)
    {
        OpenBraceToken = openBraceToken;
        CloseBraceToken = closeBraceToken;

        Children = new ISyntax[]
        {
            OpenBraceToken,
            CloseBraceToken,
        }.ToImmutableArray();
    }

    public OpenBraceToken OpenBraceToken { get; }
    public CloseBraceToken CloseBraceToken { get; }

    public bool IsFabricated { get; init; }
    public ImmutableArray<ISyntax> Children { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundObjectInitializationNode;
}
