using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a function.
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

        SetChildList();
    }

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public List<FunctionParameterEntryNode> FunctionParameterEntryNodeList { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; private set; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionParametersListingNode;
    
    public FunctionParametersListingNode SetCloseParenthesisToken(CloseParenthesisToken closeParenthesisToken)
    {
    	CloseParenthesisToken = closeParenthesisToken;
    	
    	SetChildList();
    	return this;
    }
    
    public void SetChildList()
    {
    	// OpenParenthesisToken, FunctionParameterEntryNodeList.Length, CloseParenthesisToken,
    	var childCount = 
    		1 +                                     // OpenParenthesisToken,
    		FunctionParameterEntryNodeList.Count + // FunctionParameterEntryNodeList.Length,
    		1;                                      // CloseParenthesisToken,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		foreach (var item in FunctionParameterEntryNodeList)
		{
			childList[i++] = item;
		}
		childList[i++] = CloseParenthesisToken;
            
        ChildList = childList;
    }
}
