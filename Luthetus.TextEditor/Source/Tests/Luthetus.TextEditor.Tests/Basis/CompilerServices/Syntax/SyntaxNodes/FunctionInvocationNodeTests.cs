using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="FunctionInvocationNode"/>
/// </summary>
public class FunctionInvocationNodeTests
{
    /// <summary>
    /// <see cref="FunctionInvocationNode(IdentifierToken, RazorLib.CompilerServices.Syntax.SyntaxNodes.FunctionDefinitionNode?, RazorLib.CompilerServices.Syntax.SyntaxNodes.GenericParametersListingNode?, FunctionParametersListingNode)"/>
    /// <br/>----<br/>
    /// <see cref="FunctionInvocationNode.FunctionInvocationIdentifierToken"/>
    /// <see cref="FunctionInvocationNode.FunctionDefinitionNode"/>
    /// <see cref="FunctionInvocationNode.GenericParametersListingNode"/>
    /// <see cref="FunctionInvocationNode.FunctionParametersListingNode"/>
    /// <see cref="FunctionInvocationNode.ChildBag"/>
    /// <see cref="FunctionInvocationNode.IsFabricated"/>
    /// <see cref="FunctionInvocationNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = "MyMethod(3)";

        IdentifierToken functionInvocationIdentifierToken;
        {
            var functionInvocationIdentifierText = ")";
            int indexOffunctionInvocationIdentifierText = sourceText.IndexOf(functionInvocationIdentifierText);
            
            functionInvocationIdentifierToken = new IdentifierToken(new TextEditorTextSpan(
                indexOffunctionInvocationIdentifierText,
                indexOffunctionInvocationIdentifierText + functionInvocationIdentifierText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        FunctionDefinitionNode? functionDefinitionNode = null;

        GenericParametersListingNode? genericParametersListingNode = null;

        FunctionParametersListingNode functionParametersListingNode;
        {
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

            ImmutableArray<FunctionParameterEntryNode> functionParameterEntryNodes;
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

                functionParameterEntryNodes = new FunctionParameterEntryNode[] 
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

            functionParametersListingNode = new(
                openParenthesisToken,
                functionParameterEntryNodes,
                closeParenthesisToken);
        }

        var functionInvocationNode = new FunctionInvocationNode(
            functionInvocationIdentifierToken,
            functionDefinitionNode,
            genericParametersListingNode,
            functionParametersListingNode);

        Assert.Equal(functionInvocationIdentifierToken, functionInvocationNode.FunctionInvocationIdentifierToken);
        Assert.Equal(functionDefinitionNode, functionInvocationNode.FunctionDefinitionNode);
        Assert.Equal(genericParametersListingNode, functionInvocationNode.GenericParametersListingNode);
        Assert.Equal(functionParametersListingNode, functionInvocationNode.FunctionParametersListingNode);

        Assert.Equal(2, functionInvocationNode.ChildBag.Length);
        Assert.Equal(functionInvocationIdentifierToken, functionInvocationNode.ChildBag[0]);
        Assert.Equal(functionParametersListingNode, functionInvocationNode.ChildBag[1]);

        Assert.False(functionInvocationNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.FunctionInvocationNode,
            functionInvocationNode.SyntaxKind);
	}
}