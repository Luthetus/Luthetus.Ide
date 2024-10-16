using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class BinaryOperatorNode : ISyntaxNode
{
    public BinaryOperatorNode(
        TypeClauseNode leftOperandTypeClauseNode,
        ISyntaxToken operatorToken,
        TypeClauseNode rightOperandTypeClauseNode,
        TypeClauseNode resultTypeClauseNode)
    {
        LeftOperandTypeClauseNode = leftOperandTypeClauseNode;
        OperatorToken = operatorToken;
        RightOperandTypeClauseNode = rightOperandTypeClauseNode;
        ResultTypeClauseNode = resultTypeClauseNode;

        SetChildList();
    }

    public TypeClauseNode LeftOperandTypeClauseNode { get; }
    public ISyntaxToken OperatorToken { get; }
    public TypeClauseNode RightOperandTypeClauseNode { get; }
	public TypeClauseNode ResultTypeClauseNode { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BinaryOperatorNode;
    
    public void SetChildList()
    {
    	ChildList = new ISyntax[]
        {
            OperatorToken
        }
        .ToImmutableArray();
    	throw new NotImplementedException();
    }
}