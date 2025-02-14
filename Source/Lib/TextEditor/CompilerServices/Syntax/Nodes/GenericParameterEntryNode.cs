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
public sealed class GenericParameterEntryNode : IExpressionNode
{
    public GenericParameterEntryNode(TypeClauseNode typeClauseNode)
    {
        TypeClauseNode = typeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode TypeClauseNode { get; }
    TypeClauseNode IExpressionNode.ResultTypeClauseNode => TypeFacts.Pseudo.ToTypeClause();

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.GenericParameterEntryNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 1; // TypeClauseNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = TypeClauseNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}