using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ExplicitCastNode : IExpressionNode
{
	public ExplicitCastNode(
        SyntaxToken openParenthesisToken,
        TypeClauseNode resultTypeClauseNode,
        SyntaxToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        ResultTypeClauseNode = resultTypeClauseNode;
        CloseParenthesisToken = closeParenthesisToken;
    }

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;
    
    public ExplicitCastNode(SyntaxToken openParenthesisToken, TypeClauseNode resultTypeClauseNode)
    	: this(openParenthesisToken, resultTypeClauseNode, default)
    {
    }

    public SyntaxToken OpenParenthesisToken { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }
    public SyntaxToken CloseParenthesisToken { get; private set; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ExplicitCastNode;
    
    public ExplicitCastNode SetCloseParenthesisToken(SyntaxToken closeParenthesisToken)
    {
    	CloseParenthesisToken = closeParenthesisToken;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public IReadOnlyList<ISyntax> GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 3; // OpenParenthesisToken, ResultTypeClauseNode, CloseParenthesisToken,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		childList[i++] = ResultTypeClauseNode;
		childList[i++] = CloseParenthesisToken;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
