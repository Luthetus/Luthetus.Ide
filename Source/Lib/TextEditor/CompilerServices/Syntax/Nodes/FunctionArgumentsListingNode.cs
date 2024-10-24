using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a function.
/// </summary>
public sealed class FunctionArgumentsListingNode : ISyntaxNode
{
    public FunctionArgumentsListingNode(
        OpenParenthesisToken openParenthesisToken,
        ImmutableArray<FunctionArgumentEntryNode> functionArgumentEntryNodes,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        FunctionArgumentEntryNodeList = functionArgumentEntryNodes;
        CloseParenthesisToken = closeParenthesisToken;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public ImmutableArray<FunctionArgumentEntryNode> FunctionArgumentEntryNodeList { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentsListingNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		_childListIsDirty;
    	
    	// OpenParenthesisToken, FunctionArgumentEntryNodeList.Length, CloseParenthesisToken,
    	var childCount = 
    		1 +                                    // OpenParenthesisToken,
    		FunctionArgumentEntryNodeList.Length + // FunctionArgumentEntryNodeList.Length,
    		1;                                     // CloseParenthesisToken,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		foreach (var item in FunctionArgumentEntryNodeList)
		{
			childList[i++] = item;
		}
		childList[i++] = CloseParenthesisToken;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}