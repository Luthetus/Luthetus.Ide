using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="VariableAssignmentExpressionNode"/>
/// </summary>
public class VariableAssignmentExpressionNodeTests
{
    /// <summary>
    /// <see cref="VariableAssignmentExpressionNode(IdentifierToken, EqualsToken, IExpressionNode)"/>
    /// <br/>----<br/>
    /// <see cref="VariableAssignmentExpressionNode.VariableIdentifierToken"/>
    /// <see cref="VariableAssignmentExpressionNode.EqualsToken"/>
    /// <see cref="VariableAssignmentExpressionNode.ExpressionNode"/>
    /// <see cref="VariableAssignmentExpressionNode.ChildList"/>
    /// <see cref="VariableAssignmentExpressionNode.IsFabricated"/>
    /// <see cref="VariableAssignmentExpressionNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = "x = 3";

        IdentifierToken variableIdentifierToken;
        {
            var variableIdentifierText = "x";
            int indexOfVariableIdentifierText = sourceText.IndexOf(variableIdentifierText);
            
            variableIdentifierToken = new IdentifierToken(new TextEditorTextSpan(
                indexOfVariableIdentifierText,
                indexOfVariableIdentifierText + variableIdentifierText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        EqualsToken equalsToken;
        {
            var equalsText = "=";
            int indexOfEqualsText = sourceText.IndexOf(equalsText);

            equalsToken = new EqualsToken(new TextEditorTextSpan(
                indexOfEqualsText,
                indexOfEqualsText + equalsText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

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

        var variableAssignmentExpressionNode = new VariableAssignmentExpressionNode(
            variableIdentifierToken,
            equalsToken,
            expressionNode);

        Assert.Equal(variableIdentifierToken, variableAssignmentExpressionNode.VariableIdentifierToken);
        Assert.Equal(equalsToken, variableAssignmentExpressionNode.EqualsToken);
        Assert.Equal(expressionNode, variableAssignmentExpressionNode.ExpressionNode);

        Assert.Equal(3, variableAssignmentExpressionNode.ChildList.Length);
        Assert.Equal(variableIdentifierToken, variableAssignmentExpressionNode.ChildList[0]);
        Assert.Equal(equalsToken, variableAssignmentExpressionNode.ChildList[1]);
        Assert.Equal(expressionNode, variableAssignmentExpressionNode.ChildList[2]);

        Assert.False(variableAssignmentExpressionNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.VariableAssignmentExpressionNode,
            variableAssignmentExpressionNode.SyntaxKind);
	}
}
