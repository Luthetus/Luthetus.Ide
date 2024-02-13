using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="VariableReferenceNode"/>
/// </summary>
public class VariableReferenceNodeTests
{
    /// <summary>
    /// <see cref="VariableReferenceNode(IdentifierToken, VariableDeclarationNode)"/>
    /// <br/>----<br/>
    /// <see cref="VariableReferenceNode.VariableIdentifierToken"/>
    /// <see cref="VariableReferenceNode.VariableDeclarationNode"/>
    /// <see cref="VariableReferenceNode.ResultTypeClauseNode"/>
    /// <see cref="VariableReferenceNode.ChildList"/>
    /// <see cref="VariableReferenceNode.IsFabricated"/>
    /// <see cref="VariableReferenceNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = @"int x = 2;
MyMethod(x)";
        _ = sourceText; // Suppress unused variable warning

        VariableDeclarationNode variableDeclarationNode;
        {
            TypeClauseNode intTypeClauseNode;
            {
                var intTypeIdentifier = new KeywordToken(
                    TextEditorTextSpan.FabricateTextSpan("int"),
                    SyntaxKind.IntTokenKeyword);

                intTypeClauseNode = new TypeClauseNode(
                    intTypeIdentifier,
                    typeof(int),
                    null);
            }

            IdentifierToken declarationVariableIdentifierToken;
            {
                var variableIdentifierText = "value";
                int indexOfVariableIdentifierText = sourceText.IndexOf(variableIdentifierText);

                declarationVariableIdentifierToken = new IdentifierToken(new TextEditorTextSpan(
                    indexOfVariableIdentifierText,
                    indexOfVariableIdentifierText + variableIdentifierText.Length,
                    0,
                    new ResourceUri("/unitTesting.txt"),
                    sourceText));
            }

            VariableKind variableKind = VariableKind.Local;

            bool isInitialized = false;

            variableDeclarationNode = new VariableDeclarationNode(
                intTypeClauseNode,
                declarationVariableIdentifierToken,
                variableKind,
                isInitialized);
        }

        IdentifierToken referenceVariableIdentifierToken;
        {
            var variableIdentifierText = "x";
            int indexOfVariableIdentifierText = sourceText.IndexOf(variableIdentifierText);

            referenceVariableIdentifierToken = new IdentifierToken(new TextEditorTextSpan(
                indexOfVariableIdentifierText,
                indexOfVariableIdentifierText + variableIdentifierText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        var variableReferenceNode = new VariableReferenceNode(
            referenceVariableIdentifierToken,
            variableDeclarationNode);

        Assert.Equal(referenceVariableIdentifierToken, variableReferenceNode.VariableIdentifierToken);
        Assert.Equal(variableDeclarationNode, variableReferenceNode.VariableDeclarationNode);
        Assert.Equal(variableDeclarationNode.TypeClauseNode, variableReferenceNode.ResultTypeClauseNode);

        Assert.Equal(2, variableReferenceNode.ChildList.Length);
        Assert.Equal(referenceVariableIdentifierToken, variableReferenceNode.ChildList[0]);
        Assert.Equal(variableDeclarationNode, variableReferenceNode.ChildList[1]);

        Assert.False(variableReferenceNode.IsFabricated);

        Assert.Equal(
            SyntaxKind.VariableReferenceNode,
            variableReferenceNode.SyntaxKind);
	}
}
