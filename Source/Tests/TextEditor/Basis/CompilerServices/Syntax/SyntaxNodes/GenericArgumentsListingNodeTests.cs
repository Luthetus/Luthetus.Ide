using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

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
    /// <see cref="GenericArgumentsListingNode.GenericArgumentEntryNodeList"/>
    /// <see cref="GenericArgumentsListingNode.CloseAngleBracketToken"/>
    /// <see cref="GenericArgumentsListingNode.ChildList"/>
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

        ImmutableArray<GenericArgumentEntryNode> genericArgumentEntryNodeList;
        {
            var genericArgumentEntryNode = new GenericArgumentEntryNode(genericTypeClauseNode);

            genericArgumentEntryNodeList = new GenericArgumentEntryNode[]
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
            genericArgumentEntryNodeList,
            closeAngleBracketToken);

        Assert.Equal(openAngleBracketToken, genericArgumentsListingNode.OpenAngleBracketToken);
        Assert.Equal(genericArgumentEntryNodeList, genericArgumentsListingNode.GenericArgumentEntryNodeList);
        Assert.Equal(closeAngleBracketToken, genericArgumentsListingNode.CloseAngleBracketToken);

        Assert.Equal(3, genericArgumentsListingNode.ChildList.Length);
        Assert.Equal(openAngleBracketToken, genericArgumentsListingNode.ChildList[0]);
        Assert.Equal(genericArgumentEntryNodeList.Single(), genericArgumentsListingNode.ChildList[1]);
        Assert.Equal(closeAngleBracketToken, genericArgumentsListingNode.ChildList[2]);

        Assert.False(genericArgumentsListingNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.GenericArgumentsListingNode,
            genericArgumentsListingNode.SyntaxKind);
    }
}