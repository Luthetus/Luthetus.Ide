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

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public IExpressionNode InnerExpression { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }
    public TypeClauseNode ResultTypeClauseNode => InnerExpression.ResultTypeClauseNode;

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ParenthesizedExpressionNode;
    
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
