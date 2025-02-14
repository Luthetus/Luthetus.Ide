using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class AmbiguousIdentifierExpressionNode : IExpressionNode
{
    public AmbiguousIdentifierExpressionNode(
        SyntaxToken token,
        GenericParametersListingNode? genericParametersListingNode,
        TypeClauseNode resultTypeClauseNode)
    {
        Token = token;
        GenericParametersListingNode = genericParametersListingNode;
        ResultTypeClauseNode = resultTypeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public SyntaxToken Token { get; }
    public GenericParametersListingNode? GenericParametersListingNode { get; private set; }
    public TypeClauseNode ResultTypeClauseNode { get; }
    public bool FollowsMemberAccessToken { get; init; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.AmbiguousIdentifierExpressionNode;
    
    public AmbiguousIdentifierExpressionNode SetGenericParametersListingNode(GenericParametersListingNode? genericParametersListingNode)
    {
    	GenericParametersListingNode = genericParametersListingNode;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    		
    	// TODO: This method.
    	_childList = Array.Empty<ISyntax>();
    	
    	_childListIsDirty = false;
    	return _childList;
    }
}

