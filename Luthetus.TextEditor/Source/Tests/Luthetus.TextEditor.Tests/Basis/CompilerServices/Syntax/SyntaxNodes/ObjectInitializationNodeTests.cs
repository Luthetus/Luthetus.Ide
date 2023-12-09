using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

/// <summary>TODO: Correctly implement this node. For now, just skip over it when parsing.</summary>
public sealed record ObjectInitializationNodeTests
{
    public ObjectInitializationNode(OpenBraceToken openBraceToken, CloseBraceToken closeBraceToken)
    {
        OpenBraceToken = openBraceToken;
        CloseBraceToken = closeBraceToken;

        ChildBag = new ISyntax[]
        {
            OpenBraceToken,
            CloseBraceToken,
        }.ToImmutableArray();
    }

    public OpenBraceToken OpenBraceToken { get; }
    public CloseBraceToken CloseBraceToken { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationNode;
}