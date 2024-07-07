using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="ReturnStatementNode"/>
/// </summary>
public class ReturnStatementNodeTests
{
    /// <summary>
    /// <see cref="ReturnStatementNode(KeywordToken, IExpressionNode)"/>
    /// <br/>----<br/>
    /// <see cref="ReturnStatementNode.KeywordToken"/>
    /// <see cref="ReturnStatementNode.ExpressionNode"/>
    /// <see cref="ReturnStatementNode.ChildList"/>
    /// <see cref="ReturnStatementNode.IsFabricated"/>
    /// <see cref="ReturnStatementNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = @"public int MyMethod(int value)
{
	return 3;
}";

        var returnKeywordText = "3";
        int indexOfReturnKeywordText = sourceText.IndexOf(returnKeywordText);

        var returnKeywordToken = new KeywordToken(
            new TextEditorTextSpan(
                indexOfReturnKeywordText,
                indexOfReturnKeywordText + returnKeywordText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText),
            RazorLib.CompilerServices.Syntax.SyntaxKind.ReturnTokenKeyword);

        IExpressionNode expressionNode;
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

            expressionNode = new LiteralExpressionNode(
                numericLiteralToken,
                intTypeClauseNode);
        }

        var returnStatementNode = new ReturnStatementNode(
            returnKeywordToken,
            expressionNode);

        Assert.Equal(returnKeywordToken, returnStatementNode.KeywordToken);
        Assert.Equal(expressionNode, returnStatementNode.ExpressionNode);

        Assert.Equal(2, returnStatementNode.ChildList.Length);
        Assert.Equal(returnKeywordToken, returnStatementNode.ChildList[0]);
        Assert.Equal(expressionNode, returnStatementNode.ChildList[1]);

        Assert.False(returnStatementNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.ReturnStatementNode,
            returnStatementNode.SyntaxKind);
	}
}