using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="FunctionArgumentsListingNode"/>
/// </summary>
public class FunctionArgumentsListingNodeTests
{
	/// <summary>
	/// <see cref="FunctionArgumentsListingNode(RazorLib.CompilerServices.Syntax.SyntaxTokens.OpenParenthesisToken, System.Collections.Immutable.ImmutableArray{FunctionArgumentEntryNode}, RazorLib.CompilerServices.Syntax.SyntaxTokens.CloseParenthesisToken)"/>
	/// </summary>
	[Fact]
	public void Constructor()
    {
        var sourceText = @"public void MyMethod(int value)
{
}";

        //FunctionArgumentsListingNode functionArgumentsListingNode;
        //{
        //    var openParenthesisText = "(";
        //    int indexOfOpenParenthesisText = sourceText.IndexOf(openParenthesisText);
        //    var openParenthesisToken = new OpenParenthesisToken(new TextEditorTextSpan(
        //        indexOfOpenParenthesisText,
        //        indexOfOpenParenthesisText + openParenthesisText.Length,
        //        0,
        //        new ResourceUri("/unitTesting.txt"),
        //        sourceText));

        //    ImmutableArray<FunctionArgumentEntryNode> functionArgumentEntryNodeBag;
        //    {
        //        TypeClauseNode intTypeClauseNode;
        //        {
        //            var intTypeIdentifier = new KeywordToken(
        //                TextEditorTextSpan.FabricateTextSpan("int"),
        //                RazorLib.CompilerServices.Syntax.SyntaxKind.IntTokenKeyword);

        //            intTypeClauseNode = new TypeClauseNode(
        //                intTypeIdentifier,
        //                typeof(int),
        //                null);
        //        }

        //        var variableIdentifierText = "value";
        //        int indexOfVariableIdentifierText = sourceText.IndexOf(variableIdentifierText);
        //        var variableIdentifierToken = new IdentifierToken(new TextEditorTextSpan(
        //            indexOfVariableIdentifierText,
        //            indexOfVariableIdentifierText + variableIdentifierText.Length,
        //            0,
        //            new ResourceUri("/unitTesting.txt"),
        //            sourceText));

        //        var variableDeclarationNode = new VariableDeclarationNode(
        //            intTypeClauseNode,
        //            variableIdentifierToken,
        //            VariableKind.Local,
        //            false);

        //        var functionArgumentEntryText = "int value";
        //        int indexOfFunctionArgumentEntryText = sourceText.IndexOf(functionArgumentEntryText);
        //        var functionArgumentEntryNode = new FunctionArgumentEntryNode(
        //            variableDeclarationNode,
        //            false,
        //            false,
        //            false,
        //            false);

        //        functionArgumentEntryNodeBag = new FunctionArgumentEntryNode[] 
        //        {
        //            functionArgumentEntryNode
        //        }.ToImmutableArray();
        //    }

        //    var closeParenthesisText = ")";
        //    int indexOfCloseParenthesisText = sourceText.IndexOf(closeParenthesisText);
        //    var closeParenthesisToken = new CloseParenthesisToken(new TextEditorTextSpan(
        //        indexOfCloseParenthesisText,
        //        indexOfCloseParenthesisText + closeParenthesisText.Length,
        //        0,
        //        new ResourceUri("/unitTesting.txt"),
        //        sourceText));

        //    functionArgumentsListingNode = new FunctionArgumentsListingNode(
        //        openParenthesisToken,
        //        functionArgumentEntryNodeBag,
        //        closeParenthesisToken);
        //}

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

        var functionArgumentEntryText = "int value";
        int indexOfFunctionArgumentEntryText = sourceText.IndexOf(functionArgumentEntryText);

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

    /// <summary>
    /// <see cref="FunctionArgumentsListingNode.OpenParenthesisToken"/>
    /// </summary>
    [Fact]
	public void OpenParenthesisToken()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="FunctionArgumentsListingNode.FunctionArgumentEntryNodeBag"/>
	/// </summary>
	[Fact]
	public void FunctionArgumentEntryNodeBag()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="FunctionArgumentsListingNode.CloseParenthesisToken"/>
	/// </summary>
	[Fact]
	public void CloseParenthesisToken()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="FunctionArgumentsListingNode.ChildBag"/>
	/// </summary>
	[Fact]
	public void ChildBag()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="FunctionArgumentsListingNode.IsFabricated"/>
	/// </summary>
	[Fact]
	public void IsFabricated()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="FunctionArgumentsListingNode.SyntaxKind"/>
	/// </summary>
	[Fact]
	public void SyntaxKind()
	{
		throw new NotImplementedException();
	}
}