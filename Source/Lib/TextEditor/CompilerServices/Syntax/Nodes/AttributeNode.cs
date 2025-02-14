namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class AttributeNode : ISyntaxNode
{
    public AttributeNode(
        SyntaxToken openSquareBracketToken,
        List<SyntaxToken> innerTokens,
        SyntaxToken closeSquareBracketToken)
    {
        OpenSquareBracketToken = openSquareBracketToken;
        InnerTokens = innerTokens;
        CloseSquareBracketToken = closeSquareBracketToken;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public SyntaxToken OpenSquareBracketToken { get; }
    public List<SyntaxToken> InnerTokens { get; }
    public SyntaxToken CloseSquareBracketToken { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.AttributeNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
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
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}