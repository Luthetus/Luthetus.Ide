using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="GenericParameterEntryNode"/>
/// </summary>
public class GenericParameterEntryNodeTests
{
    /// <summary>
    /// <see cref="GenericParameterEntryNode(TypeClauseNode)"/>
    /// <br/>----<br/>
	/// <see cref="GenericParameterEntryNode.TypeClauseNode"/>
    /// <see cref="GenericParameterEntryNode.ChildList"/>
    /// <see cref="GenericParameterEntryNode.IsFabricated"/>
    /// <see cref="GenericParameterEntryNode.SyntaxKind"/>
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

        var genericParameterEntryNode = new GenericParameterEntryNode(genericTypeClauseNode);

        Assert.Equal(genericTypeClauseNode, genericParameterEntryNode.TypeClauseNode);

        Assert.Single(genericParameterEntryNode.ChildList);
        Assert.Equal(genericTypeClauseNode, genericParameterEntryNode.ChildList.Single());

        Assert.False(genericParameterEntryNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.GenericParameterEntryNode,
            genericParameterEntryNode.SyntaxKind);
    }
}