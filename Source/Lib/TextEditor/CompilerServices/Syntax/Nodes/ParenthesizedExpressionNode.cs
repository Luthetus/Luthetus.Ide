using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ParenthesizedExpressionNode : IExpressionNode
{
    public ParenthesizedExpressionNode(
        OpenParenthesisToken openParenthesisToken,
        IExpressionNode innerExpression,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        InnerExpression = innerExpression;
        CloseParenthesisToken = closeParenthesisToken;
        
        SetChildList();
    }
    
    public ParenthesizedExpressionNode(OpenParenthesisToken openParenthesisToken, TypeClauseNode typeClauseNode)
    	: this(openParenthesisToken, new EmptyExpressionNode(typeClauseNode), default)
    {
    }

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public IExpressionNode InnerExpression { get; private set; }
    public CloseParenthesisToken CloseParenthesisToken { get; private set; }
    public TypeClauseNode ResultTypeClauseNode => InnerExpression.ResultTypeClauseNode;

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ParenthesizedExpressionNode;
    
    public ParenthesizedExpressionNode SetCloseParenthesisToken(CloseParenthesisToken closeParenthesisToken)
    {
    	CloseParenthesisToken = closeParenthesisToken;
    	SetChildList();
    	return this;
    }
    
    public ParenthesizedExpressionNode SetInnerExpression(IExpressionNode innerExpression)
    {
    	InnerExpression = innerExpression;
    	SetChildList();
    	return this;
    }
    
    public void SetChildList()
    {
    	var childCount = 4; // OpenParenthesisToken, InnerExpression, CloseParenthesisToken, ResultTypeClauseNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		childList[i++] = InnerExpression;
		childList[i++] = CloseParenthesisToken;
		childList[i++] = ResultTypeClauseNode;
            
        ChildList = childList;
    }
}
