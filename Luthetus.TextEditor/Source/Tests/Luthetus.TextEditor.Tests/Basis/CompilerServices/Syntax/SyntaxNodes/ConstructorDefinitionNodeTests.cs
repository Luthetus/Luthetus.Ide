using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="ConstructorDefinitionNode"/>
/// </summary>
public class ConstructorDefinitionNodeTests
{
    /// <summary>
    /// <see cref="ConstructorDefinitionNode(TypeClauseNode, IdentifierToken, RazorLib.CompilerServices.Syntax.SyntaxNodes.GenericArgumentsListingNode?, FunctionArgumentsListingNode, RazorLib.CompilerServices.CodeBlockNode?, RazorLib.CompilerServices.Syntax.SyntaxNodes.ConstraintNode?)"/>
    /// <br/>----<br/>
    /// <see cref="ConstructorDefinitionNode.ReturnTypeClauseNode"/>
    /// <see cref="ConstructorDefinitionNode.FunctionIdentifier"/>
    /// <see cref="ConstructorDefinitionNode.GenericArgumentsListingNode"/>
    /// <see cref="ConstructorDefinitionNode.FunctionArgumentsListingNode"/>
    /// <see cref="ConstructorDefinitionNode.FunctionBodyCodeBlockNode"/>
    /// <see cref="ConstructorDefinitionNode.ConstraintNode"/>
    /// <see cref="ConstructorDefinitionNode.ChildBag"/>
    /// <see cref="ConstructorDefinitionNode.IsFabricated"/>
    /// <see cref="ConstructorDefinitionNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var sourceText = @"public class MyClass
{
    public MyClass()
    {
    }
}";

        TypeClauseNode myClassTypeClauseNode;
        {
            var myClassTypeIdentifier = new IdentifierToken(
                TextEditorTextSpan.FabricateTextSpan("MyClass"));

            myClassTypeClauseNode = new TypeClauseNode(
                myClassTypeIdentifier,
                null,
                null);
        }
        
        IdentifierToken functionIdentifierToken;
        {
            functionIdentifierToken = new IdentifierToken(
                TextEditorTextSpan.FabricateTextSpan("MyClass"));
        }

        FunctionArgumentsListingNode functionArgumentsListingNode;
        {
            var openParenthesisText = "(";
            int indexOfOpenParenthesisText = sourceText.IndexOf(openParenthesisText);
            var openParenthesisToken = new OpenParenthesisToken(new TextEditorTextSpan(
                indexOfOpenParenthesisText,
                indexOfOpenParenthesisText + openParenthesisText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));

            var functionArgumentEntryNodes = ImmutableArray<FunctionArgumentEntryNode>.Empty;

            var closeParenthesisText = ")";
            int indexOfCloseParenthesisText = sourceText.IndexOf(closeParenthesisText);
            var closeParenthesisToken = new CloseParenthesisToken(new TextEditorTextSpan(
                indexOfCloseParenthesisText,
                indexOfCloseParenthesisText + closeParenthesisText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));

            functionArgumentsListingNode = new FunctionArgumentsListingNode(
                openParenthesisToken,
                functionArgumentEntryNodes,
                closeParenthesisToken);
        }

        GenericArgumentsListingNode? genericArgumentsListingNode = null;

        CodeBlockNode functionBodyCodeBlockNode;
        {
            functionBodyCodeBlockNode = new CodeBlockNode(
                ImmutableArray<RazorLib.CompilerServices.Syntax.ISyntax>.Empty);
        }

        ConstraintNode? constraintNode = null;

        var constructorDefinitionNode = new ConstructorDefinitionNode(
            myClassTypeClauseNode,
            functionIdentifierToken,
            genericArgumentsListingNode,
            functionArgumentsListingNode,
            functionBodyCodeBlockNode,
            constraintNode);

        Assert.Equal(myClassTypeClauseNode, constructorDefinitionNode.ReturnTypeClauseNode);
        Assert.Equal(functionIdentifierToken, constructorDefinitionNode.FunctionIdentifier);
        Assert.Equal(genericArgumentsListingNode, constructorDefinitionNode.GenericArgumentsListingNode);
        Assert.Equal(functionArgumentsListingNode, constructorDefinitionNode.FunctionArgumentsListingNode);
        Assert.Equal(functionBodyCodeBlockNode, constructorDefinitionNode.FunctionBodyCodeBlockNode);
        Assert.Equal(constraintNode, constructorDefinitionNode.ConstraintNode);

        Assert.Equal(4, constructorDefinitionNode.ChildBag.Length);
        Assert.Equal(myClassTypeClauseNode, constructorDefinitionNode.ChildBag[0]);
        Assert.Equal(functionIdentifierToken, constructorDefinitionNode.ChildBag[1]);
        Assert.Equal(functionArgumentsListingNode, constructorDefinitionNode.ChildBag[2]);
        Assert.Equal(functionBodyCodeBlockNode, constructorDefinitionNode.ChildBag[3]);

        Assert.False(constructorDefinitionNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.ConstructorDefinitionNode,
            constructorDefinitionNode.SyntaxKind);
    }
}
