using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="ParenthesizedExpressionNode"/>
/// </summary>
public class ParenthesizedExpressionNodeTests
{
    /// <summary>
    /// <see cref="ParenthesizedExpressionNode(OpenParenthesisToken, IExpressionNode, CloseParenthesisToken)"/>
    /// <br/>----<br/>
    /// <see cref="ParenthesizedExpressionNode.OpenParenthesisToken"/>
    /// <see cref="ParenthesizedExpressionNode.InnerExpression"/>
    /// <see cref="ParenthesizedExpressionNode.CloseParenthesisToken"/>
    /// <see cref="ParenthesizedExpressionNode.ResultTypeClauseNode"/>
    /// <see cref="ParenthesizedExpressionNode.ChildList"/>
    /// <see cref="ParenthesizedExpressionNode.IsFabricated"/>
    /// <see cref="ParenthesizedExpressionNode.SyntaxKind"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var sourceText = "(3)";

        OpenParenthesisToken openParenthesisToken;
        {
            var openParenthesisText = "(";
            int indexOfOpenParenthesisText = sourceText.IndexOf(openParenthesisText);
            openParenthesisToken = new OpenParenthesisToken(new TextEditorTextSpan(
                indexOfOpenParenthesisText,
                indexOfOpenParenthesisText + openParenthesisText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        IExpressionNode innerExpression;
        {
            var numericLiteralText = "3";
            int indexOfNumericLiteralText = sourceText.IndexOf(numericLiteralText);
            var numericLiteralToken = new NumericLiteralToken(new TextEditorTextSpan(
                indexOfNumericLiteralText,
                indexOfNumericLiteralText + numericLiteralText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));

            TypeClauseNode intTypeClauseNode;
            {
                var intTypeIdentifier = new KeywordToken(
                    TextEditorTextSpan.FabricateTextSpan("int"),
                    RazorLib.CompilerServices.Syntax.SyntaxKind.IntTokenKeyword);

                intTypeClauseNode = new TypeClauseNode(
                    intTypeIdentifier,
                    typeof(int),
                    null);
            }

            innerExpression = new LiteralExpressionNode(
                numericLiteralToken,
                intTypeClauseNode);
        }

        CloseParenthesisToken closeParenthesisToken;
        {
            var closeParenthesisText = ")";
            int indexOfCloseParenthesisText = sourceText.IndexOf(closeParenthesisText);
            closeParenthesisToken = new CloseParenthesisToken(new TextEditorTextSpan(
                indexOfCloseParenthesisText,
                indexOfCloseParenthesisText + closeParenthesisText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        var parenthesizedExpressionNode = new ParenthesizedExpressionNode(
            openParenthesisToken,
            innerExpression,
            closeParenthesisToken);

        Assert.Equal(openParenthesisToken, parenthesizedExpressionNode.OpenParenthesisToken);
        Assert.Equal(innerExpression, parenthesizedExpressionNode.InnerExpression);
        Assert.Equal(closeParenthesisToken, parenthesizedExpressionNode.CloseParenthesisToken);
        Assert.Equal(innerExpression.ResultTypeClauseNode, parenthesizedExpressionNode.ResultTypeClauseNode);

        Assert.Equal(4, parenthesizedExpressionNode.ChildList.Length);
        Assert.Equal(openParenthesisToken, parenthesizedExpressionNode.ChildList[0]);
        Assert.Equal(innerExpression, parenthesizedExpressionNode.ChildList[1]);
        Assert.Equal(closeParenthesisToken, parenthesizedExpressionNode.ChildList[2]);
        Assert.Equal(innerExpression.ResultTypeClauseNode, parenthesizedExpressionNode.ChildList[3]);

        Assert.False(parenthesizedExpressionNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.ParenthesizedExpressionNode,
            parenthesizedExpressionNode.SyntaxKind);
	}
}
