using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="VariableDeclarationNode"/>
/// </summary>
public class VariableDeclarationNodeTests
{
    /// <summary>
    /// <see cref="VariableDeclarationNode(TypeClauseNode, IdentifierToken, VariableKind, bool)"/>
    /// <br/>----<br/>
	/// <see cref="VariableDeclarationNode.TypeClauseNode"/>
    /// <see cref="VariableDeclarationNode.IdentifierToken"/>
    /// <see cref="VariableDeclarationNode.VariableKind"/>
    /// <see cref="VariableDeclarationNode.IsInitialized"/>
    /// <see cref="VariableDeclarationNode.HasGetter"/>
    /// <see cref="VariableDeclarationNode.GetterIsAutoImplemented"/>
    /// <see cref="VariableDeclarationNode.HasSetter"/>
    /// <see cref="VariableDeclarationNode.SetterIsAutoImplemented"/>
    /// <see cref="VariableDeclarationNode.ChildBag"/>
    /// <see cref="VariableDeclarationNode.IsFabricated"/>
    /// <see cref="VariableDeclarationNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = "int x;";

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

        IdentifierToken variableIdentifierToken;
        {
            var variableIdentifierText = "value";
            int indexOfVariableIdentifierText = sourceText.IndexOf(variableIdentifierText);

            variableIdentifierToken = new IdentifierToken(new TextEditorTextSpan(
                indexOfVariableIdentifierText,
                indexOfVariableIdentifierText + variableIdentifierText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        VariableKind variableKind = VariableKind.Local;

        bool isInitialized = false;

        var variableDeclarationNode = new VariableDeclarationNode(
            intTypeClauseNode,
            variableIdentifierToken,
            variableKind,
            isInitialized);

        Assert.Equal(intTypeClauseNode, variableDeclarationNode.TypeClauseNode);
        Assert.Equal(variableIdentifierToken, variableDeclarationNode.IdentifierToken);
        Assert.Equal(variableKind, variableDeclarationNode.VariableKind);
        Assert.Equal(isInitialized, variableDeclarationNode.IsInitialized);

        Assert.Equal(2, variableDeclarationNode.ChildBag.Length);
        Assert.Equal(intTypeClauseNode, variableDeclarationNode.ChildBag[0]);
        Assert.Equal(variableIdentifierToken, variableDeclarationNode.ChildBag[1]);

        Assert.False(variableDeclarationNode.IsFabricated);

        Assert.Equal(
            SyntaxKind.VariableDeclarationNode,
            variableDeclarationNode.SyntaxKind);
	}
}
