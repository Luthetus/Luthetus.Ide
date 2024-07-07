using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="BinaryExpressionNode"/>
/// </summary>
public class BinaryExpressionNodeTests
{
    /// <summary>
    /// <see cref="BinaryExpressionNode(IExpressionNode, BinaryOperatorNode, IExpressionNode)"/>
    /// <br/>----<br/>
    /// <see cref="BinaryExpressionNode.LeftExpressionNode"/>
    /// <see cref="BinaryExpressionNode.BinaryOperatorNode"/>
    /// <see cref="BinaryExpressionNode.RightExpressionNode"/>
    /// <see cref="BinaryExpressionNode.ResultTypeClauseNode"/>
    /// <see cref="BinaryExpressionNode.ChildList"/>
    /// <see cref="BinaryExpressionNode.IsFabricated"/>
    /// <see cref="BinaryExpressionNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var leftExpressionText = "3";
        var binaryOperatorText = "+";
        var rightExpressionText = "7";
        var binaryExpressionText = $"{leftExpressionText} {binaryOperatorText} {rightExpressionText}";
        var sourceText = $@"SomeMethodInvocation({binaryExpressionText})";

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

        IExpressionNode leftExpressionNode;
        {
            var leftExpressionNodeInclusiveStartIndex = sourceText.IndexOf(leftExpressionText);

            var leftNumericLiteralToken = new NumericLiteralToken(new TextEditorTextSpan(
                leftExpressionNodeInclusiveStartIndex,
                leftExpressionNodeInclusiveStartIndex + leftExpressionText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));

            leftExpressionNode = new LiteralExpressionNode(
                leftNumericLiteralToken,
                intTypeClauseNode);
        }

        BinaryOperatorNode binaryOperatorNode;
        {
            var plusTokenInclusiveStartIndex = sourceText.IndexOf(binaryOperatorText);

            var plusToken = new PlusToken(new TextEditorTextSpan(
                plusTokenInclusiveStartIndex,
                plusTokenInclusiveStartIndex + binaryOperatorText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));

            binaryOperatorNode = new BinaryOperatorNode(
                intTypeClauseNode,
                plusToken,
                intTypeClauseNode,
                intTypeClauseNode);
        }

        IExpressionNode rightExpressionNode;
        {
            var rightExpressionNodeInclusiveStartIndex = sourceText.IndexOf(rightExpressionText);

            var rightNumericLiteralToken = new NumericLiteralToken(new TextEditorTextSpan(
                rightExpressionNodeInclusiveStartIndex,
                rightExpressionNodeInclusiveStartIndex + rightExpressionText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));

            rightExpressionNode = new LiteralExpressionNode(
                rightNumericLiteralToken,
                intTypeClauseNode);
        }

        var binaryExpressionNode = new BinaryExpressionNode(
            leftExpressionNode,
            binaryOperatorNode,
            rightExpressionNode);

        Assert.Equal(leftExpressionNode, binaryExpressionNode.LeftExpressionNode);
        Assert.Equal(binaryOperatorNode, binaryExpressionNode.BinaryOperatorNode);
        Assert.Equal(rightExpressionNode, binaryExpressionNode.RightExpressionNode);
        Assert.Equal(binaryOperatorNode.ResultTypeClauseNode, binaryExpressionNode.ResultTypeClauseNode);

        Assert.Equal(3, binaryExpressionNode.ChildList.Length);
        Assert.Equal(leftExpressionNode, binaryExpressionNode.ChildList[0]);
        Assert.Equal(binaryOperatorNode, binaryExpressionNode.ChildList[1]);
        Assert.Equal(rightExpressionNode, binaryExpressionNode.ChildList[2]);

        Assert.False(binaryExpressionNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.BinaryExpressionNode,
            binaryExpressionNode.SyntaxKind);
    }
}