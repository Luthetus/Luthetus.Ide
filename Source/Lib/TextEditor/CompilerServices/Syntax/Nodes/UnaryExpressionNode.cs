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

        SetChildList();
    }

    public IExpressionNode Expression { get; }
    public UnaryOperatorNode UnaryOperatorNode { get; }
    public TypeClauseNode ResultTypeClauseNode => UnaryOperatorNode.ResultTypeClauseNode;

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UnaryExpressionNode;
    
    public void SetChildList()
    {
    	ChildList = new ISyntax[]
        {
            Expression,
            UnaryOperatorNode,
        };
    }
}