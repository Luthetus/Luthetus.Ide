using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ParenthesizedExpressionNode : IExpressionNode
{
    public ParenthesizedExpressionNode(
        SyntaxToken openParenthesisToken,
        IExpressionNode innerExpression,
        SyntaxToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        InnerExpression = innerExpression;
        CloseParenthesisToken = closeParenthesisToken;
    }

	public ParenthesizedExpressionNode(SyntaxToken openParenthesisToken, TypeClauseNode typeClauseNode)
    	: this(openParenthesisToken, new EmptyExpressionNode(typeClauseNode), default)
    {
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;
    
    public SyntaxToken OpenParenthesisToken { get; }
    public IExpressionNode InnerExpression { get; private set; }
    public SyntaxToken CloseParenthesisToken { get; private set; }
    public TypeClauseNode ResultTypeClauseNode => InnerExpression.ResultTypeClauseNode;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ParenthesizedExpressionNode;
    
    public ParenthesizedExpressionNode SetCloseParenthesisToken(SyntaxToken closeParenthesisToken)
    {
    	CloseParenthesisToken = closeParenthesisToken;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public ParenthesizedExpressionNode SetInnerExpression(IExpressionNode innerExpression)
    {
    	InnerExpression = innerExpression;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 4; // OpenParenthesisToken, InnerExpression, CloseParenthesisToken, ResultTypeClauseNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		childList[i++] = InnerExpression;
		childList[i++] = CloseParenthesisToken;
		childList[i++] = ResultTypeClauseNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
