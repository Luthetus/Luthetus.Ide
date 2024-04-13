using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>TODO: Correctly implement this node. For now, just skip over it when parsing.</summary>
public sealed record ObjectInitializationNode : ISyntaxNode
{
    public ObjectInitializationNode(OpenBraceToken openBraceToken, CloseBraceToken closeBraceToken)
    {
        OpenBraceToken = openBraceToken;
        CloseBraceToken = closeBraceToken;

        ChildList = new ISyntax[]
        {
            OpenBraceToken,
            CloseBraceToken,
        }.ToImmutableArray();
    }

    public OpenBraceToken OpenBraceToken { get; }
    public CloseBraceToken CloseBraceToken { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationNode;
}