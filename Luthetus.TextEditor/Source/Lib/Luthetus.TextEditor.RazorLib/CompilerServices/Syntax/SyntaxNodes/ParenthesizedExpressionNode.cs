using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record ParenthesizedExpressionNode : IExpressionNode
{
    public ParenthesizedExpressionNode(
        OpenParenthesisToken openParenthesisToken,
        IExpressionNode innerExpression,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        InnerExpression = innerExpression;
        CloseParenthesisToken = closeParenthesisToken;

        var children = new List<ISyntax>
        {
            OpenParenthesisToken,
            InnerExpression,
            CloseParenthesisToken,
            ResultTypeClauseNode,
        };

        ChildList = children.ToImmutableArray();
    }

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public IExpressionNode InnerExpression { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }
    public TypeClauseNode ResultTypeClauseNode => InnerExpression.ResultTypeClauseNode;

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ParenthesizedExpressionNode;
}
