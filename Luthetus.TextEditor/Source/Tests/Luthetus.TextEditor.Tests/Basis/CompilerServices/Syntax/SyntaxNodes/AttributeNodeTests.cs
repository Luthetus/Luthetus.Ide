using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="AttributeNode"/>
/// </summary>
public class AttributeNodeTests
{
    /// <summary>
    /// <see cref="AttributeNode(OpenSquareBracketToken, CloseSquareBracketToken)"/>
    /// <br/>----<br/>
	/// <see cref="AttributeNode.OpenSquareBracketToken"/>
	/// <see cref="AttributeNode.CloseSquareBracketToken"/>
	/// <see cref="AttributeNode.ChildList"/>
	/// <see cref="AttributeNode.IsFabricated"/>
	/// <see cref="AttributeNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var attributeNodeText = "[Parameter, EditorRequired]";
        var sourceText = $@"public partial class ContextBoundary : ComponentBase
{{
    {attributeNodeText}
    public ContextRecord ContextRecord {{ get; set; }} = null!;
}}";

        OpenSquareBracketToken openSquareBracketToken;
        {
            var openSquareBracketTokenText = "[";
            var openSquareBracketTokenInclusiveStartIndex = sourceText.IndexOf(openSquareBracketTokenText);

            openSquareBracketToken = new OpenSquareBracketToken(new TextEditorTextSpan(
                openSquareBracketTokenInclusiveStartIndex,
                openSquareBracketTokenInclusiveStartIndex + openSquareBracketTokenText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        CloseSquareBracketToken closeSquareBracketToken;
        {
            var closeSquareBracketTokenText = "]";
            var closeSquareBracketTokenInclusiveStartIndex = sourceText.IndexOf(closeSquareBracketTokenText);

            closeSquareBracketToken = new CloseSquareBracketToken(new TextEditorTextSpan(
                closeSquareBracketTokenInclusiveStartIndex,
                closeSquareBracketTokenInclusiveStartIndex + closeSquareBracketTokenText.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                sourceText));
        }

        var attributeNode = new AttributeNode(
            openSquareBracketToken,
            // This new list is a hack so the tests compile.
            new(), 
            closeSquareBracketToken);

        Assert.Equal(openSquareBracketToken, attributeNode.OpenSquareBracketToken);
        Assert.Equal(closeSquareBracketToken, attributeNode.CloseSquareBracketToken);

        Assert.Equal(2, attributeNode.ChildList.Length);
        Assert.Equal(openSquareBracketToken, attributeNode.ChildList[0]);
        Assert.Equal(closeSquareBracketToken, attributeNode.ChildList[1]);

        Assert.False(attributeNode.IsFabricated);

        Assert.Equal(
            RazorLib.CompilerServices.Syntax.SyntaxKind.AttributeNode,
            attributeNode.SyntaxKind);
    }
}