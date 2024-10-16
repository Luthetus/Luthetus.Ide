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
    	// OpenSquareBracketToken, InnerTokens.Count, CloseSquareBracketToken
    	var childCount = 
    		1 +                 // OpenSquareBracketToken,
    		InnerTokens.Count + // InnerTokens.Count,
    		1;                  // CloseSquareBracketToken,
    		
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenSquareBracketToken;
		foreach (var item in InnerTokens)
		{
			childList[i++] = item;
		}
		childList[i++] = CloseSquareBracketToken;
            
        ChildList = childList;
    }
}