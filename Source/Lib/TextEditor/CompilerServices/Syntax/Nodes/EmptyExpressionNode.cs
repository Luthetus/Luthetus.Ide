using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// One usage of the <see cref="EmptyExpressionNode"/> is for a <see cref="ParenthesizedExpressionNode"/>
/// which has no <see cref="ParenthesizedExpressionNode.InnerExpression"/>
/// </summary>
public sealed class EmptyExpressionNode : IExpressionNode
{
    public EmptyExpressionNode(TypeClauseNode typeClauseNode)
    {
        ResultTypeClauseNode = typeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode ResultTypeClauseNode { get; }

    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.EmptyExpressionNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 1; // ResultTypeClauseNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = ResultTypeClauseNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}