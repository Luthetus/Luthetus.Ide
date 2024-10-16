using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class AttributeNode : ISyntaxNode
{
    public AttributeNode(
        OpenSquareBracketToken openSquareBracketToken,
        List<ISyntaxToken> innerTokens,
        CloseSquareBracketToken closeSquareBracketToken)
    {
        OpenSquareBracketToken = openSquareBracketToken;
        InnerTokens = innerTokens;
        CloseSquareBracketToken = closeSquareBracketToken;

        SetChildList();
    }

    public OpenSquareBracketToken OpenSquareBracketToken { get; }
    public List<ISyntaxToken> InnerTokens { get; }
    public CloseSquareBracketToken CloseSquareBracketToken { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.AttributeNode;
    
    public void SetChildList()
    {
    	var childList = new List<ISyntax>
        {
            OpenSquareBracketToken
        };

        childList.AddRange(innerTokens);
        childList.Add(CloseSquareBracketToken);
        
        ChildList = childList.ToImmutableArray();
    	throw new NotImplementedException();
    }
}