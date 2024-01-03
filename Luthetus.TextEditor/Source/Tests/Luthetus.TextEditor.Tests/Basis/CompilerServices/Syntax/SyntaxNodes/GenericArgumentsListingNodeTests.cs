using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="GenericArgumentsListingNode"/>
/// </summary>
public class GenericArgumentsListingNodeTests
{
    /// <summary>
    /// <see cref="GenericArgumentsListingNode(OpenAngleBracketToken, ImmutableArray{GenericArgumentEntryNode}, CloseAngleBracketToken)"/>
    /// <br/>----<br/>
    /// <see cref="GenericArgumentsListingNode.OpenAngleBracketToken"/>
    /// <see cref="GenericArgumentsListingNode.GenericArgumentEntryNodeBag"/>
    /// <see cref="GenericArgumentsListingNode.CloseAngleBracketToken"/>
    /// <see cref="GenericArgumentsListingNode.ChildBag"/>
    /// <see cref="GenericArgumentsListingNode.IsFabricated"/>
    /// <see cref="GenericArgumentsListingNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = @"public void AddData<TItem>(TItem data)
{
}";

        TypeClauseNode genericTypeClauseNode;
        {
            var genericArgumentEntryText = "TItem";
            int indexOfGenericArgumentEntryText = sourceText.IndexOf(genericArgumentEntryText);
            var genericArgumentIdentifierToken = new IdentifierToken(new TextEditorTextSpan(
                indexOfGenericArgumentEntryText,
                indexOfGenericArgumentEntryText + genericArgumentEntryText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));

            genericTypeClauseNode = new TypeClauseNode(
                genericArgumentIdentifierToken,
                null,
                null);
        }

        OpenAngleBracketToken openAngleBracketToken;
        {
            var openAngleBracketText = "<";
            int indexOfOpenAngleBracketText = sourceText.IndexOf(openAngleBracketText);
            openAngleBracketToken = new OpenAngleBracketToken(new TextEditorTextSpan(
                indexOfOpenAngleBracketText,
                indexOfOpenAngleBracketText + openAngleBracketText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        ImmutableArray<GenericArgumentEntryNode> genericArgumentEntryNodeBag;
        {
            var genericArgumentEntryNode = new GenericArgumentEntryNode(genericTypeClauseNode);

            genericArgumentEntryNodeBag = new GenericArgumentEntryNode[]
            {
                    genericArgumentEntryNode
            }.ToImmutableArray();
        }

        CloseAngleBracketToken closeAngleBracketToken;
        {
            var closeAngleBracketText = ">";
            int indexOfCloseAngleBracketText = sourceText.IndexOf(closeAngleBracketText);
            closeAngleBracketToken = new CloseAngleBracketToken(new TextEditorTextSpan(
                indexOfCloseAngleBracketText,
                indexOfCloseAngleBracketText + closeAngleBracketText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        var genericArgumentsListingNode = new GenericArgumentsListingNode(
            openAngleBracketToken,
            genericArgumentEntryNodeBag,
            closeAngleBracketToken);

        Assert.Equal(openAngleBracketToken, genericArgumentsListingNode.OpenAngleBracketToken);
        Assert.Equal(genericArgumentEntryNodeBag, genericArgumentsListingNode.GenericArgumentEntryNodeBag);
        Assert.Equal(closeAngleBracketToken, genericArgumentsListingNode.CloseAngleBracketToken);

        Assert.Equal(3, genericArgumentsListingNode.ChildBag.Length);
        Assert.Equal(openAngleBracketToken, genericArgumentsListingNode.ChildBag[0]);
        Assert.Equal(genericArgumentEntryNodeBag.Single(), genericArgumentsListingNode.ChildBag[1]);
        Assert.Equal(closeAngleBracketToken, genericArgumentsListingNode.ChildBag[2]);

        Assert.False(genericArgumentsListingNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.GenericArgumentsListingNode,
            genericArgumentsListingNode.SyntaxKind);
    }
}