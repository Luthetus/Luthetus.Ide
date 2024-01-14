using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="BinaryOperatorNode"/>
/// </summary>
public class BinaryOperatorNodeTests
{
    /// <summary>
    /// <see cref="BinaryOperatorNode(TypeClauseNode, RazorLib.CompilerServices.Syntax.ISyntaxToken, TypeClauseNode, TypeClauseNode)"/>
    /// <br/>----<br/>
    /// <see cref="BinaryOperatorNode.LeftOperandTypeClauseNode"/>
    /// <see cref="BinaryOperatorNode.OperatorToken"/>
    /// <see cref="BinaryOperatorNode.RightOperandTypeClauseNode"/>
    /// <see cref="BinaryOperatorNode.ResultTypeClauseNode"/>
    /// <see cref="BinaryOperatorNode.ChildList"/>
    /// <see cref="BinaryOperatorNode.IsFabricated"/>
    /// <see cref="BinaryOperatorNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var binaryOperatorText = "+";
        var sourceText = $@"{binaryOperatorText}";

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

        PlusToken plusToken;
        {
            var plusTokenInclusiveStartIndex = sourceText.IndexOf(binaryOperatorText);

            plusToken = new PlusToken(new TextEditorTextSpan(
                plusTokenInclusiveStartIndex,
                plusTokenInclusiveStartIndex + binaryOperatorText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        var binaryOperatorNode = new BinaryOperatorNode(
            intTypeClauseNode,
            plusToken,
            intTypeClauseNode,
            intTypeClauseNode);

        Assert.Equal(intTypeClauseNode, binaryOperatorNode.LeftOperandTypeClauseNode);
        Assert.Equal(plusToken, binaryOperatorNode.OperatorToken);
        Assert.Equal(intTypeClauseNode, binaryOperatorNode.RightOperandTypeClauseNode);
        Assert.Equal(intTypeClauseNode, binaryOperatorNode.ResultTypeClauseNode);

        Assert.Single(binaryOperatorNode.ChildList);
        Assert.Equal(plusToken, binaryOperatorNode.ChildList.Single());

        Assert.False(binaryOperatorNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.BinaryOperatorNode,
            binaryOperatorNode.SyntaxKind);
    }
}