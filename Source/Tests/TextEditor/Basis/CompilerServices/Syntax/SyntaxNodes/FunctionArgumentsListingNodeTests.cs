using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="FunctionArgumentsListingNode"/>
/// </summary>
public class FunctionArgumentsListingNodeTests
{
    /// <summary>
    /// <see cref="FunctionArgumentsListingNode(OpenParenthesisToken, ImmutableArray{FunctionArgumentEntryNode}, CloseParenthesisToken)"/>
    /// <br/>----<br/>
    /// <see cref="FunctionArgumentsListingNode.OpenParenthesisToken"/>
    /// <see cref="FunctionArgumentsListingNode.FunctionArgumentEntryNodeList"/>
    /// <see cref="FunctionArgumentsListingNode.CloseParenthesisToken"/>
    /// <see cref="FunctionArgumentsListingNode.ChildList"/>
    /// <see cref="FunctionArgumentsListingNode.IsFabricated"/>
    /// <see cref="FunctionArgumentsListingNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var sourceText = @"public void MyMethod(int value)
{
}";

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

        ImmutableArray<FunctionArgumentEntryNode> functionArgumentEntryNodeList;
        {
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

            var variableIdentifierText = "value";
            int indexOfVariableIdentifierText = sourceText.IndexOf(variableIdentifierText);
            var variableIdentifierToken = new IdentifierToken(new TextEditorTextSpan(
                indexOfVariableIdentifierText,
                indexOfVariableIdentifierText + variableIdentifierText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));

            var variableDeclarationNode = new VariableDeclarationNode(
                intTypeClauseNode,
                variableIdentifierToken,
                VariableKind.Local,
                false);

            var functionArgumentEntryNode = new FunctionArgumentEntryNode(
                variableDeclarationNode,
                false,
                false,
                false,
                false);

            functionArgumentEntryNodeList = new FunctionArgumentEntryNode[]
            {
                    functionArgumentEntryNode
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

        var functionArgumentsListingNode = new FunctionArgumentsListingNode(
            openParenthesisToken,
            functionArgumentEntryNodeList,
            closeParenthesisToken);

        Assert.Equal(openParenthesisToken, functionArgumentsListingNode.OpenParenthesisToken);
        Assert.Equal(functionArgumentEntryNodeList, functionArgumentsListingNode.FunctionArgumentEntryNodeList);
        Assert.Equal(closeParenthesisToken, functionArgumentsListingNode.CloseParenthesisToken);

        Assert.Equal(3, functionArgumentsListingNode.ChildList.Length);
        Assert.Equal(openParenthesisToken, functionArgumentsListingNode.ChildList[0]);
        Assert.Equal(functionArgumentEntryNodeList.Single(), functionArgumentsListingNode.ChildList[1]);
        Assert.Equal(closeParenthesisToken, functionArgumentsListingNode.ChildList[2]);

        Assert.False(functionArgumentsListingNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.FunctionArgumentsListingNode,
            functionArgumentsListingNode.SyntaxKind);
    }
}