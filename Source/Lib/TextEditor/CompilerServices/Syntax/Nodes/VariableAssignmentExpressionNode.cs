using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class VariableAssignmentExpressionNode : ISyntaxNode
{
    public VariableAssignmentExpressionNode(
        IdentifierToken variableIdentifierToken,
        EqualsToken equalsToken,
        IExpressionNode expressionNode)
    {
        VariableIdentifierToken = variableIdentifierToken;
        EqualsToken = equalsToken;
        ExpressionNode = expressionNode;

        SetChildList();
    }

    public IdentifierToken VariableIdentifierToken { get; }
    public EqualsToken EqualsToken { get; }
    public IExpressionNode ExpressionNode { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableAssignmentExpressionNode;
    
    public void SetChildList()
    {
    	ChildList = new ISyntax[]
        {
            VariableIdentifierToken,
            EqualsToken,
            ExpressionNode,
        };
    }
}
