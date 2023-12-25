using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="FunctionArgumentEntryNode"/>
/// </summary>
public class FunctionArgumentEntryNodeTests
{
    /// <summary>
    /// <see cref="FunctionArgumentEntryNode(VariableDeclarationNode, bool, bool, bool, bool)"/>
    /// <br/>----<br/>
    /// <see cref="FunctionArgumentEntryNode.VariableDeclarationNode"/>
    /// <see cref="FunctionArgumentEntryNode.IsOptional"/>
    /// <see cref="FunctionArgumentEntryNode.HasOutKeyword"/>
    /// <see cref="FunctionArgumentEntryNode.HasInKeyword"/>
    /// <see cref="FunctionArgumentEntryNode.HasRefKeyword"/>
    /// <see cref="FunctionArgumentEntryNode.ChildBag"/>
    /// <see cref="FunctionArgumentEntryNode.IsFabricated"/>
    /// <see cref="FunctionArgumentEntryNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = @"public void MyMethod(int value)
{
}";

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

        var isOptional = false;
        var hasOutKeyword = false;
        var hasInKeyword = false;
        var hasRefKeyword = false;

        var functionArgumentEntryNode = new FunctionArgumentEntryNode(
            variableDeclarationNode,
            isOptional,
            hasOutKeyword,
            hasInKeyword,
            hasRefKeyword);

        Assert.Equal(variableDeclarationNode, functionArgumentEntryNode.VariableDeclarationNode);
        Assert.Equal(isOptional, functionArgumentEntryNode.IsOptional);
        Assert.Equal(hasOutKeyword, functionArgumentEntryNode.HasOutKeyword);
        Assert.Equal(hasInKeyword, functionArgumentEntryNode.HasInKeyword);
        Assert.Equal(hasRefKeyword, functionArgumentEntryNode.HasRefKeyword);

        Assert.Single(functionArgumentEntryNode.ChildBag);
        Assert.Equal(variableDeclarationNode, functionArgumentEntryNode.ChildBag.Single());

        Assert.False(functionArgumentEntryNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.FunctionArgumentEntryNode,
            functionArgumentEntryNode.SyntaxKind);
    }
}