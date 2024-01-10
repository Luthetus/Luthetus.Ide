using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="FunctionParameterEntryNode"/>
/// </summary>
public class FunctionParameterEntryNodeTests
{
    /// <summary>
    /// <see cref="FunctionParameterEntryNode(IExpressionNode, bool, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="FunctionParameterEntryNode.ExpressionNode"/>
    /// <see cref="FunctionParameterEntryNode.HasOutKeyword"/>
    /// <see cref="FunctionParameterEntryNode.HasInKeyword"/>
    /// <see cref="FunctionParameterEntryNode.HasRefKeyword"/>
    /// <see cref="FunctionParameterEntryNode.ChildList"/>
    /// <see cref="FunctionParameterEntryNode.IsFabricated"/>
    /// <see cref="FunctionParameterEntryNode.SyntaxKind.FunctionParameterEntryNode"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = "MyMethod(3)";

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

        var expressionNode = new LiteralExpressionNode(
            numericLiteralToken,
            intTypeClauseNode);

        var hasOutKeyword = false;
        var hasInKeyword = false;
        var hasRefKeyword = false;

        var functionParameterEntryNode = new FunctionParameterEntryNode(
            expressionNode,
            hasOutKeyword,
            hasInKeyword,
            hasRefKeyword);

        Assert.Equal(expressionNode, functionParameterEntryNode.ExpressionNode);
        Assert.Equal(hasOutKeyword, functionParameterEntryNode.HasOutKeyword);
        Assert.Equal(hasInKeyword, functionParameterEntryNode.HasInKeyword);
        Assert.Equal(hasRefKeyword, functionParameterEntryNode.HasRefKeyword);

        Assert.Single(functionParameterEntryNode.ChildList);
        Assert.Equal(expressionNode, functionParameterEntryNode.ChildList.Single());

        Assert.False(functionParameterEntryNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.FunctionParameterEntryNode,
            functionParameterEntryNode.SyntaxKind);
	}
}