using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class InheritanceStatementNode : ISyntaxNode
{
    public InheritanceStatementNode(TypeClauseNode parentTypeClauseNode)
    {
        ParentTypeClauseNode = parentTypeClauseNode;

        SetChildList();
    }

    public TypeClauseNode ParentTypeClauseNode { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.InheritanceStatementNode;
    
    public void SetChildList()
    {
    	var childCount = 1; // ParentTypeClauseNode
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ParentTypeClauseNode;
            
        ChildList = childList;
    }
}