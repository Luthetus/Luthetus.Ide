using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class InheritanceStatementNode : ISyntaxNode
{
    public InheritanceStatementNode(TypeClauseNode parentTypeClauseNode)
    {
        ParentTypeClauseNode = parentTypeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode ParentTypeClauseNode { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.InheritanceStatementNode;
    
    public int GetStartInclusiveIndex()
    {
    }
    
    public int GetEndExclusiveIndex()
    {
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 1; // ParentTypeClauseNode
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ParentTypeClauseNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}