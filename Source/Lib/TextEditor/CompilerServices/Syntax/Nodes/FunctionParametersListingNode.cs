using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a function.
///
/// TODO: I don't like how this type has the 'OpenParenthesisToken, and CloseParenthesisToken'...
///       ...It was done this way in order to mirror the generic parameters.
///       |
/// 	  Because for generic parameters, the 'OpenAngleBracketToken, and CloseAngleBracketToken'
///       are tied to the existance of generic parameters
///       (i.e.: you must match at least 1 generic parameter if the 'OpenAngleBracketToken' is there.).
///       |
///       With the function parameters however, the 'OpenParenthesisToken' does not
///       mandate that at least 1 function parameter must be matched.
/// </summary>
public sealed class FunctionParametersListingNode : ISyntaxNode
{
    public FunctionParametersListingNode(
        OpenParenthesisToken openParenthesisToken,
        List<FunctionParameterEntryNode> functionParameterEntryNodes,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        FunctionParameterEntryNodeList = functionParameterEntryNodes;
        CloseParenthesisToken = closeParenthesisToken;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public List<FunctionParameterEntryNode> FunctionParameterEntryNodeList { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; private set; }

    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionParametersListingNode;
    
    public FunctionParametersListingNode SetCloseParenthesisToken(CloseParenthesisToken closeParenthesisToken)
    {
    	CloseParenthesisToken = closeParenthesisToken;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	// OpenParenthesisToken, FunctionParameterEntryNodeList.Length, CloseParenthesisToken,
    	var childCount = 
    		1 +                                     // OpenParenthesisToken,
    		FunctionParameterEntryNodeList.Count + // FunctionParameterEntryNodeList.Count,
    		1;                                      // CloseParenthesisToken,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		foreach (var item in FunctionParameterEntryNodeList)
		{
			childList[i++] = item;
		}
		childList[i++] = CloseParenthesisToken;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
