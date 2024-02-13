using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="GenericArgumentEntryNode"/>
/// </summary>
public class GenericArgumentEntryNodeTests
{
    /// <summary>
    /// <see cref="GenericArgumentEntryNode(TypeClauseNode)"/>
    /// <br/>----<br/>
    /// <see cref="GenericArgumentEntryNode.TypeClauseNode"/>
    /// <see cref="GenericArgumentEntryNode.ChildList"/>
    /// <see cref="GenericArgumentEntryNode.IsFabricated"/>
    /// <see cref="GenericArgumentEntryNode.SyntaxKind"/>
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

        var genericArgumentEntryNode = new GenericArgumentEntryNode(genericTypeClauseNode);

        Assert.Equal(genericTypeClauseNode, genericArgumentEntryNode.TypeClauseNode);

        Assert.Single(genericArgumentEntryNode.ChildList);
        Assert.Equal(genericTypeClauseNode, genericArgumentEntryNode.ChildList.Single());

        Assert.False(genericArgumentEntryNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.GenericArgumentEntryNode,
            genericArgumentEntryNode.SyntaxKind);
	}
}