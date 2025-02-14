using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ReturnStatementNode : IExpressionNode
{
    public ReturnStatementNode(SyntaxToken keywordToken, IExpressionNode expressionNode)
    {
        KeywordToken = keywordToken;
        ExpressionNode = expressionNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public SyntaxToken KeywordToken { get; }
    public IExpressionNode ExpressionNode { get; }
    public TypeClauseNode ResultTypeClauseNode => ExpressionNode.ResultTypeClauseNode;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ReturnStatementNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 2; // KeywordToken, ExpressionNode
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = KeywordToken;
		childList[i++] = ExpressionNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}