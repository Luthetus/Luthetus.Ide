using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class UnaryOperatorNode : ISyntaxNode
{
    public UnaryOperatorNode(
        TypeClauseNode operandTypeClauseNode,
        ISyntaxToken operatorToken,
        TypeClauseNode resultTypeClauseNode)
    {
        OperandTypeClauseNode = operandTypeClauseNode;
        OperatorToken = operatorToken;
        ResultTypeClauseNode = resultTypeClauseNode;

        SetChildList();
    }

    public TypeClauseNode OperandTypeClauseNode { get; }
    public ISyntaxToken OperatorToken { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.UnaryOperatorNode;
    
    public void SetChildList()
    {
    	ChildList = new ISyntax[]
        {
            OperandTypeClauseNode,
            OperatorToken,
            ResultTypeClauseNode,
        };
    }
}