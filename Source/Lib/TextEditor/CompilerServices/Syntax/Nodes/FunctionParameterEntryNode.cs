using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a function.
///
/// I'm going to experiment with making this a <see cref="IExpressionNode"/>.
/// Because the parameters are exclusive to the expression parsing logic,
/// and having to wrap this in a 'BadExpressionNode' when dealing with
/// expressions is very hard to read. (2024-10-26)
/// </summary>
public sealed class FunctionParameterEntryNode : IExpressionNode
{
    public FunctionParameterEntryNode(
        IExpressionNode expressionNode,
        bool hasOutKeyword,
        bool hasInKeyword,
        bool hasRefKeyword)
    {
        ExpressionNode = expressionNode;
        HasOutKeyword = hasOutKeyword;
        HasInKeyword = hasInKeyword;
        HasRefKeyword = hasRefKeyword;
    }

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public IExpressionNode ExpressionNode { get; }
    public bool HasOutKeyword { get; }
    public bool HasInKeyword { get; }
    public bool HasRefKeyword { get; }
    TypeClauseNode IExpressionNode.ResultTypeClauseNode => TypeFacts.Pseudo.ToTypeClause();

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionParameterEntryNode;
    
    public IReadOnlyList<ISyntax> GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 1; // ExpressionNode
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ExpressionNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
