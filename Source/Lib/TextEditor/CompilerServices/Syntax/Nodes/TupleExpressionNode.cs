using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class TupleExpressionNode : IExpressionNode
{
    public TupleExpressionNode()
    {
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode ResultTypeClauseNode { get; } = TypeFacts.Empty.ToTypeClause();
    
    public List<IExpressionNode> InnerExpressionList { get; } = new();

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TupleExpressionNode;
    
    public void AddInnerExpressionNode(IExpressionNode expressionNode)
    {
    	InnerExpressionList.Add(expressionNode);
    	_childListIsDirty = true;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = InnerExpressionList.ToArray();
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
