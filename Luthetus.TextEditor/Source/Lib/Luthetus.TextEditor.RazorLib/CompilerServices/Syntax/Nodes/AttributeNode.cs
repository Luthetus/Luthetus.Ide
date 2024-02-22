using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record AttributeNode : ISyntaxNode
{
    public AttributeNode(
        OpenSquareBracketToken openSquareBracketToken,
        List<ISyntaxToken> innerTokens,
        CloseSquareBracketToken closeSquareBracketToken)
    {
        OpenSquareBracketToken = openSquareBracketToken;
        InnerTokens = innerTokens;
        CloseSquareBracketToken = closeSquareBracketToken;

        var childList = new List<ISyntax>
        {
            OpenSquareBracketToken
        };

        childList.AddRange(innerTokens);
        childList.Add(CloseSquareBracketToken);
        
        ChildList = childList.ToImmutableArray();
    }

    public OpenSquareBracketToken OpenSquareBracketToken { get; }
    public List<ISyntaxToken> InnerTokens { get; }
    public CloseSquareBracketToken CloseSquareBracketToken { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.AttributeNode;
}