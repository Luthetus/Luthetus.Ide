using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a syntax which contains a generic type.
///
/// I'm going to experiment with making this a <see cref="IExpressionNode"/>.
/// Because the parameters are exclusive to the expression parsing logic,
/// and having to wrap this in a 'BadExpressionNode' when dealing with
/// expressions is very hard to read. (2024-10-26)
/// </summary>
public sealed class GenericParametersListingNode : IExpressionNode
{
    public GenericParametersListingNode(
        SyntaxToken openAngleBracketToken,
        List<GenericParameterEntryNode> genericParameterEntryNodeList,
        SyntaxToken closeAngleBracketToken)
    {
        OpenAngleBracketToken = openAngleBracketToken;
        GenericParameterEntryNodeList = genericParameterEntryNodeList;
        CloseAngleBracketToken = closeAngleBracketToken;
    }

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public SyntaxToken OpenAngleBracketToken { get; }
    public List<GenericParameterEntryNode> GenericParameterEntryNodeList { get; }
    public SyntaxToken CloseAngleBracketToken { get; private set; }
    TypeClauseNode IExpressionNode.ResultTypeClauseNode => TypeFacts.Pseudo.ToTypeClause();

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.GenericParametersListingNode;
    
    public GenericParametersListingNode SetCloseAngleBracketToken(SyntaxToken closeAngleBracketToken)
    {
    	CloseAngleBracketToken = closeAngleBracketToken;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public IReadOnlyList<ISyntax> GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	// OpenAngleBracketToken, GenericParameterEntryNodeList.Length, CloseAngleBracketToken,
    	var childCount = 
    		1 +                                    // OpenAngleBracketToken,
    		GenericParameterEntryNodeList.Count + // GenericParameterEntryNodeList.Count,
    		1;                                     // CloseAngleBracketToken,
    	
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenAngleBracketToken;
		foreach (var item in GenericParameterEntryNodeList)
		{
			childList[i++] = item;
		}
		childList[i++] = CloseAngleBracketToken;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}