using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="FunctionDefinitionNode"/>
/// </summary>
public class FunctionDefinitionNodeTests
{
    /// <summary>
    /// <see cref="FunctionDefinitionNode(TypeClauseNode, IdentifierToken, RazorLib.CompilerServices.Syntax.SyntaxNodes.GenericArgumentsListingNode?, RazorLib.CompilerServices.Syntax.SyntaxNodes.FunctionArgumentsListingNode, RazorLib.CompilerServices.CodeBlockNode?, RazorLib.CompilerServices.Syntax.SyntaxNodes.ConstraintNode?)"/>
    /// <br/>----<br/>
    /// <see cref="FunctionDefinitionNode.ReturnTypeClauseNode"/>
    /// <see cref="FunctionDefinitionNode.FunctionIdentifier"/>
    /// <see cref="FunctionDefinitionNode.GenericArgumentsListingNode"/>
    /// <see cref="FunctionDefinitionNode.FunctionArgumentsListingNode"/>
    /// <see cref="FunctionDefinitionNode.FunctionBodyCodeBlockNode"/>
    /// <see cref="FunctionDefinitionNode.ConstraintNode"/>
    /// <see cref="FunctionDefinitionNode.ChildBag"/>
    /// <see cref="FunctionDefinitionNode.IsFabricated"/>
    /// <see cref="FunctionDefinitionNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var sourceText = @"public void MyMethod(int value)
{
}";

        TypeClauseNode returnTypeClauseNode;
        {
            var returnTypeClauseText = "void";
            int indexOfReturnTypeClauseText = sourceText.IndexOf(returnTypeClauseText);
            var returnTypeClauseToken = new KeywordToken(new TextEditorTextSpan(
                indexOfReturnTypeClauseText,
                indexOfReturnTypeClauseText + returnTypeClauseText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText),
                RazorLib.CompilerServices.Syntax.SyntaxKind.VoidTokenKeyword);

            returnTypeClauseNode = new TypeClauseNode(returnTypeClauseToken, typeof(void), null);
        }
        
        IdentifierToken functionIdentifierToken;
        {
            var functionIdentifierText = "MyMethod";
            int indexOfFunctionIdentifierText = sourceText.IndexOf(functionIdentifierText);

            functionIdentifierToken = new IdentifierToken(new TextEditorTextSpan(
                indexOfFunctionIdentifierText,
                indexOfFunctionIdentifierText + functionIdentifierText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        GenericArgumentsListingNode? genericArgumentsListingNode = null;

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

        ImmutableArray<FunctionArgumentEntryNode> functionArgumentEntryNodeBag;
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

            functionArgumentEntryNodeBag = new FunctionArgumentEntryNode[]
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

        FunctionArgumentsListingNode functionArgumentsListingNode;
        {
            functionArgumentsListingNode = new FunctionArgumentsListingNode(
                openParenthesisToken,
                functionArgumentEntryNodeBag,
                closeParenthesisToken);
        }

        CodeBlockNode codeBlockNode = new CodeBlockNode(
            ImmutableArray<RazorLib.CompilerServices.Syntax.ISyntax>.Empty);
        
        ConstraintNode? constraintNode = null;

        var functionDefinitionNode = new FunctionDefinitionNode(
            returnTypeClauseNode,
            functionIdentifierToken,
            genericArgumentsListingNode,
            functionArgumentsListingNode,
            codeBlockNode,
            constraintNode);

        Assert.Equal(returnTypeClauseNode, functionDefinitionNode.ReturnTypeClauseNode);
        Assert.Equal(functionIdentifierToken, functionDefinitionNode.FunctionIdentifier);
        Assert.Equal(genericArgumentsListingNode, functionDefinitionNode.GenericArgumentsListingNode);
        Assert.Equal(functionArgumentsListingNode, functionDefinitionNode.FunctionArgumentsListingNode);
        Assert.Equal(codeBlockNode, functionDefinitionNode.FunctionBodyCodeBlockNode);
        Assert.Equal(constraintNode, functionDefinitionNode.ConstraintNode);

        Assert.Equal(4, functionDefinitionNode.ChildBag.Length);
        Assert.Equal(returnTypeClauseNode, functionDefinitionNode.ChildBag[0]);
        Assert.Equal(functionIdentifierToken, functionDefinitionNode.ChildBag[1]);
        Assert.Equal(functionArgumentsListingNode, functionDefinitionNode.ChildBag[2]);
        Assert.Equal(codeBlockNode, functionDefinitionNode.ChildBag[3]);

        Assert.False(functionDefinitionNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.FunctionDefinitionNode,
            functionDefinitionNode.SyntaxKind);
    }
}
