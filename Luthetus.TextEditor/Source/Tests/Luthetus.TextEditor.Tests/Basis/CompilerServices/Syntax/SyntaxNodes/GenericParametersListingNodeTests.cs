using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
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
    /// <see cref="GenericParametersListingNode.GenericParameterEntryNodeList"/>
    /// <see cref="GenericParametersListingNode.CloseAngleBracketToken"/>
    /// <see cref="GenericParametersListingNode.ChildList"/>
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

        ImmutableArray<GenericParameterEntryNode> genericParameterEntryNodeList;
        {
            var genericParameterEntryNode = new GenericParameterEntryNode(genericTypeClauseNode);

            genericParameterEntryNodeList = new GenericParameterEntryNode[]
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
            genericParameterEntryNodeList,
            closeAngleBracketToken);

        Assert.Equal(openAngleBracketToken, genericParametersListingNode.OpenAngleBracketToken);
        Assert.Equal(genericParameterEntryNodeList, genericParametersListingNode.GenericParameterEntryNodeList);
        Assert.Equal(closeAngleBracketToken, genericParametersListingNode.CloseAngleBracketToken);

        Assert.Equal(3, genericParametersListingNode.ChildList.Length);
        Assert.Equal(openAngleBracketToken, genericParametersListingNode.ChildList[0]);
        Assert.Equal(genericParameterEntryNodeList.Single(), genericParametersListingNode.ChildList[1]);
        Assert.Equal(closeAngleBracketToken, genericParametersListingNode.ChildList[2]);

        Assert.False(genericParametersListingNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.GenericParametersListingNode,
            genericParametersListingNode.SyntaxKind);
	}
}