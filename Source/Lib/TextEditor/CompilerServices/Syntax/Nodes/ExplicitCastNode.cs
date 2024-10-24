using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ExplicitCastNode : IExpressionNode
{
	public ExplicitCastNode(
        OpenParenthesisToken openParenthesisToken,
        TypeClauseNode resultTypeClauseNode,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        ResultTypeClauseNode = resultTypeClauseNode;
        CloseParenthesisToken = closeParenthesisToken;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;
    
    public ExplicitCastNode(OpenParenthesisToken openParenthesisToken, TypeClauseNode resultTypeClauseNode)
    	: this(openParenthesisToken, resultTypeClauseNode, default)
    {
    }

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; private set; }

    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ExplicitCastNode;
    
    public ExplicitCastNode SetCloseParenthesisToken(CloseParenthesisToken closeParenthesisToken)
    {
    	CloseParenthesisToken = closeParenthesisToken;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public ISyntax[] GetChildList()
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
