using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="TypeClauseNode"/>
/// </summary>
public class TypeClauseNodeTests
{
    /// <summary>
    /// <see cref="TypeClauseNode(RazorLib.CompilerServices.Syntax.ISyntaxToken, Type?, RazorLib.CompilerServices.Syntax.SyntaxNodes.GenericParametersListingNode?)"/>
    /// <br/>----<br/>
    /// <see cref="TypeClauseNode.TypeIdentifier"/>
    /// <see cref="TypeClauseNode.ValueType"/>
    /// <see cref="TypeClauseNode.GenericParametersListingNode"/>
    /// <see cref="TypeClauseNode.ChildList"/>
    /// <see cref="TypeClauseNode.IsFabricated"/>
    /// <see cref="TypeClauseNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var sourceText = @"public void MyMethod(int value)
{
}";
        _ = sourceText; // Suppress unused variable

        var intTypeIdentifier = new KeywordToken(
            TextEditorTextSpan.FabricateTextSpan("int"),
            RazorLib.CompilerServices.Syntax.SyntaxKind.IntTokenKeyword);

        var valueType = typeof(int);

        GenericParametersListingNode? genericParametersListingNode = null;

        var intTypeClauseNode = new TypeClauseNode(
            intTypeIdentifier,
            valueType,
            genericParametersListingNode);

        Assert.Equal(intTypeIdentifier, intTypeClauseNode.TypeIdentifier);
        Assert.Equal(valueType, intTypeClauseNode.ValueType);
        Assert.Equal(genericParametersListingNode, intTypeClauseNode.GenericParametersListingNode);

        Assert.Single(intTypeClauseNode.ChildList);
        Assert.Equal(intTypeIdentifier, intTypeClauseNode.ChildList.Single());

        Assert.False(intTypeClauseNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.TypeClauseNode,
            intTypeClauseNode.SyntaxKind);
	}
}