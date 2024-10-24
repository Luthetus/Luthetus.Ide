using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class UnaryExpressionNode : IExpressionNode
{
    public UnaryExpressionNode(
        IExpressionNode expression,
        UnaryOperatorNode unaryOperatorNode)
    {
        Expression = expression;
        UnaryOperatorNode = unaryOperatorNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public IExpressionNode Expression { get; }
    public UnaryOperatorNode UnaryOperatorNode { get; }
    public TypeClauseNode ResultTypeClauseNode => UnaryOperatorNode.ResultTypeClauseNode;

    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UnaryExpressionNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = new ISyntax[]
        {
            Expression,
            UnaryOperatorNode,
        };
        
    	_childListIsDirty = false;
    	return _childList;
    }
}