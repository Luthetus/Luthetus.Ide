using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="GenericParametersListingNode"/>
/// </summary>
public class GenericParametersListingNodeTests
{
    /// <summary>
    /// <see cref="GenericParametersListingNode(OpenAngleBracketToken, ImmutableArray{GenericParameterEntryNode}, CloseAngleBracketToken)"/>
    /// <br/>----<br/>
    /// <see cref="GenericParametersListingNode.OpenAngleBracketToken"/>
    /// <see cref="GenericParametersListingNode.GenericParameterEntryNodeBag"/>
    /// <see cref="GenericParametersListingNode.CloseAngleBracketToken"/>
    /// <see cref="GenericParametersListingNode.ChildBag"/>
    /// <see cref="GenericParametersListingNode.IsFabricated"/>
    /// <see cref="GenericParametersListingNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = @"public void AddData<TItem>(TItem data)
{
}";

        TypeClauseNode genericTypeClauseNode;
        {
            var genericParameterEntryText = "TItem";
            int indexOfGenericParameterEntryText = sourceText.IndexOf(genericParameterEntryText);
            var genericParameterIdentifierToken = new IdentifierToken(new TextEditorTextSpan(
                indexOfGenericParameterEntryText,
                indexOfGenericParameterEntryText + genericParameterEntryText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));

            genericTypeClauseNode = new TypeClauseNode(
                genericParameterIdentifierToken,
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

        ImmutableArray<GenericParameterEntryNode> genericParameterEntryNodeBag;
        {
            var genericParameterEntryNode = new GenericParameterEntryNode(genericTypeClauseNode);

            genericParameterEntryNodeBag = new GenericParameterEntryNode[]
            {
                genericParameterEntryNode
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

        var genericParametersListingNode = new GenericParametersListingNode(
            openAngleBracketToken,
            genericParameterEntryNodeBag,
            closeAngleBracketToken);

        Assert.Equal(openAngleBracketToken, genericParametersListingNode.OpenAngleBracketToken);
        Assert.Equal(genericParameterEntryNodeBag, genericParametersListingNode.GenericParameterEntryNodeBag);
        Assert.Equal(closeAngleBracketToken, genericParametersListingNode.CloseAngleBracketToken);

        Assert.Equal(3, genericParametersListingNode.ChildBag.Length);
        Assert.Equal(openAngleBracketToken, genericParametersListingNode.ChildBag[0]);
        Assert.Equal(genericParameterEntryNodeBag.Single(), genericParametersListingNode.ChildBag[1]);
        Assert.Equal(closeAngleBracketToken, genericParametersListingNode.ChildBag[2]);

        Assert.False(genericParametersListingNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.GenericParametersListingNode,
            genericParametersListingNode.SyntaxKind);
	}
}