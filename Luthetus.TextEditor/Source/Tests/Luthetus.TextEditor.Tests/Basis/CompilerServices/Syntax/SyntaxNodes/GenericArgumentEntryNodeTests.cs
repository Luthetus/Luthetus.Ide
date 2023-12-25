using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Enums;

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
    /// <see cref="GenericArgumentEntryNode.ChildBag"/>
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

        Assert.Single(genericArgumentEntryNode.ChildBag);
        Assert.Equal(genericTypeClauseNode, genericArgumentEntryNode.ChildBag.Single());

        Assert.False(genericArgumentEntryNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.GenericArgumentEntryNode,
            genericArgumentEntryNode.SyntaxKind);
	}
}