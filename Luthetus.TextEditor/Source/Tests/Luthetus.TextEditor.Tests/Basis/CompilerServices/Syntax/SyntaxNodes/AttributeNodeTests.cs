using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record AttributeNodeTests
{
    public AttributeNode(
        OpenSquareBracketToken openSquareBracketToken,
        CloseSquareBracketToken closeSquareBracketToken)
    {
        OpenSquareBracketToken = openSquareBracketToken;
        CloseSquareBracketToken = closeSquareBracketToken;

        ChildBag = new ISyntax[]
        {
            OpenSquareBracketToken,
            CloseSquareBracketToken,
        }.ToImmutableArray();
    }

    public OpenSquareBracketToken OpenSquareBracketToken { get; }
    public CloseSquareBracketToken CloseSquareBracketToken { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.AttributeNode;
}