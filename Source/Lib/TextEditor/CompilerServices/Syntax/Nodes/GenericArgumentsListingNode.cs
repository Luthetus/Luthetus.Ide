using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a syntax which contains a generic type.
/// </summary>
public sealed class GenericArgumentsListingNode : ISyntaxNode
{
    public GenericArgumentsListingNode(
        SyntaxToken openAngleBracketToken,
        ImmutableArray<GenericArgumentEntryNode> genericArgumentEntryNodeList,
        SyntaxToken closeAngleBracketToken)
    {
        OpenAngleBracketToken = openAngleBracketToken;
        GenericArgumentEntryNodeList = genericArgumentEntryNodeList;
        CloseAngleBracketToken = closeAngleBracketToken;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public SyntaxToken OpenAngleBracketToken { get; }
    public ImmutableArray<GenericArgumentEntryNode> GenericArgumentEntryNodeList { get; }
    public SyntaxToken CloseAngleBracketToken { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.GenericArgumentsListingNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	// OpenAngleBracketToken, GenericArgumentEntryNodeList.Length, CloseAngleBracketToken
    	var childCount =
    		1 +                                   // OpenAngleBracketToken,
    		GenericArgumentEntryNodeList.Length + // GenericArgumentEntryNodeList.Length,
    		1;                                    // CloseAngleBracketToken
    		
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenAngleBracketToken;
		foreach (var item in GenericArgumentEntryNodeList)
		{
			childList[i++] = item;
		}
		childList[i++] = CloseAngleBracketToken;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}