using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record ParenthesizedExpressionNodeTests
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
            TypeClauseNode,
        };

        ChildBag = children.ToImmutableArray();
    }

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public IExpressionNode InnerExpression { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }
    public TypeClauseNode TypeClauseNode => InnerExpression.TypeClauseNode;

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ParenthesizedExpressionNode;
}
