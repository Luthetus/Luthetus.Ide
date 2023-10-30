using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

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

        ChildBag = new ISyntax[]
        {
            VariableIdentifierToken,
            EqualsToken,
            ExpressionNode,
        }.ToImmutableArray();
    }

    public IdentifierToken VariableIdentifierToken { get; }
    public EqualsToken EqualsToken { get; }
    public IExpressionNode ExpressionNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableAssignmentExpressionNode;
}
