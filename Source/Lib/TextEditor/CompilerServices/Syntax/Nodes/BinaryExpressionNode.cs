using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class BinaryExpressionNode : IExpressionNode
{
    public BinaryExpressionNode(
        IExpressionNode leftExpressionNode,
        BinaryOperatorNode binaryOperatorNode,
        IExpressionNode rightExpressionNode)
    {
        LeftExpressionNode = leftExpressionNode;
        BinaryOperatorNode = binaryOperatorNode;
        RightExpressionNode = rightExpressionNode;

        SetChildList();
    }
    
    public BinaryExpressionNode(
	        IExpressionNode leftExpressionNode,
	        BinaryOperatorNode binaryOperatorNode)
        : this(leftExpressionNode, binaryOperatorNode, new EmptyExpressionNode(binaryOperatorNode.RightOperandTypeClauseNode))
    {
    }

    public IExpressionNode LeftExpressionNode { get; }
    public BinaryOperatorNode BinaryOperatorNode { get; }
    public IExpressionNode RightExpressionNode { get; private set; }
    public TypeClauseNode ResultTypeClauseNode => BinaryOperatorNode.ResultTypeClauseNode;

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BinaryExpressionNode;
    
    public void SetChildList()
    {
    	var childCount = 3; // LeftExpressionNode, BinaryOperatorNode, RightExpressionNode
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = LeftExpressionNode;
		childList[i++] = BinaryOperatorNode;
		childList[i++] = RightExpressionNode;
            
        ChildList = childList;
    }
    
    public BinaryExpressionNode SetRightExpressionNode(IExpressionNode rightExpressionNode)
    {
    	RightExpressionNode = rightExpressionNode;
    	SetChildList();
    	
    	return this;
    }
}