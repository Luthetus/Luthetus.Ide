using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="FunctionParametersListingNode"/>
/// </summary>
public class FunctionParametersListingNodeTests
{
    /// <summary>
    /// <see cref="FunctionParametersListingNode(OpenParenthesisToken, ImmutableArray{FunctionParameterEntryNode}, CloseParenthesisToken)"/>
    /// <br/>----<br/>
    /// <see cref="FunctionParametersListingNode.OpenParenthesisToken"/>
    /// <see cref="FunctionParametersListingNode.FunctionParameterEntryNodeList"/>
    /// <see cref="FunctionParametersListingNode.CloseParenthesisToken"/>
    /// <see cref="FunctionParametersListingNode.ChildList"/>
    /// <see cref="FunctionParametersListingNode.IsFabricated"/>
    /// <see cref="FunctionParametersListingNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = "MyMethod(3)";

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

        ImmutableArray<FunctionParameterEntryNode> functionParameterEntryNodeList;
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

            var literalExpressionNode = new LiteralExpressionNode(
                numericLiteralToken,
                intTypeClauseNode);

            var functionParameterEntryNode = new FunctionParameterEntryNode(
                literalExpressionNode,
                false,
                false,
                false);

            functionParameterEntryNodeList = new FunctionParameterEntryNode[]
            {
                    functionParameterEntryNode
            }.ToImmutableArray();
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

        var functionParametersListingNode = new FunctionParametersListingNode(
            openParenthesisToken,
            functionParameterEntryNodeList,
            closeParenthesisToken);

        Assert.Equal(openParenthesisToken, functionParametersListingNode.OpenParenthesisToken);
        Assert.Equal(functionParameterEntryNodeList, functionParametersListingNode.FunctionParameterEntryNodeList);
        Assert.Equal(closeParenthesisToken, functionParametersListingNode.CloseParenthesisToken);

        Assert.Equal(3, functionParametersListingNode.ChildList.Length);
        Assert.Equal(openParenthesisToken, functionParametersListingNode.ChildList[0]);
        Assert.Equal(functionParameterEntryNodeList.Single(), functionParametersListingNode.ChildList[1]);
        Assert.Equal(closeParenthesisToken, functionParametersListingNode.ChildList[2]);

        Assert.False(functionParametersListingNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.FunctionParametersListingNode,
            functionParametersListingNode.SyntaxKind);
	}
}