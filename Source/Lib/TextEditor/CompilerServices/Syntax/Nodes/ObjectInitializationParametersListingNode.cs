using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ObjectInitializationParametersListingNode : ISyntaxNode
{
    public ObjectInitializationParametersListingNode(
        OpenBraceToken openBraceToken,
        List<ObjectInitializationParameterEntryNode> objectInitializationParameterEntryNodeList,
        CloseBraceToken closeBraceToken)
    {
        OpenBraceToken = openBraceToken;
        ObjectInitializationParameterEntryNodeList = objectInitializationParameterEntryNodeList;
        CloseBraceToken = closeBraceToken;

        SetChildList();
    }

    public OpenBraceToken OpenBraceToken { get; }
    public List<ObjectInitializationParameterEntryNode> ObjectInitializationParameterEntryNodeList { get; }
    public CloseBraceToken CloseBraceToken { get; private set; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationParametersListingNode;
    
    public ObjectInitializationParametersListingNode SetCloseBraceToken(CloseBraceToken closeBraceToken)
    {
    	CloseBraceToken = closeBraceToken;
    	
    	SetChildList();
    	return this;
    }
    
    public void SetChildList()
    {
    	// OpenBraceToken, ObjectInitializationParameterEntryNodeList.Length, CloseBraceToken
    	var childCount = 
    		1 +                                                // OpenBraceToken
    		ObjectInitializationParameterEntryNodeList.Count + // ObjectInitializationParameterEntryNodeList.Count
    		1;                                                 // CloseBraceToken
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenBraceToken;
		foreach (var item in ObjectInitializationParameterEntryNodeList)
		{
			childList[i++] = item;
		}
		childList[i++] = CloseBraceToken;
            
        ChildList = childList;
    }
}
