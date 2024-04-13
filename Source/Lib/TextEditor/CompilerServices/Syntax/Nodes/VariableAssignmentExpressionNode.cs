using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record VariableAssignmentExpressionNode : ISyntaxNode
{
    public VariableAssignmentExpressionNode(
        IdentifierToken variableIdentifierToken,
        EqualsToken equalsToken,
        IExpressionNode expressionNode)
    {
        VariableIdentifierToken = variableIdentifierToken;
        EqualsToken = equalsToken;
        ExpressionNode = expressionNode;

        ChildList = new ISyntax[]
        {
            VariableIdentifierToken,
            EqualsToken,
            ExpressionNode,
        }.ToImmutableArray();
    }

    public IdentifierToken VariableIdentifierToken { get; }
    public EqualsToken EqualsToken { get; }
    public IExpressionNode ExpressionNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableAssignmentExpressionNode;
}
