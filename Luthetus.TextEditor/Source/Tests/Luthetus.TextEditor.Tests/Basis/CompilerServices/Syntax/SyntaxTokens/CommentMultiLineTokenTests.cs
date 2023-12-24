using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="CommentMultiLineToken"/>
/// </summary>
public sealed record CommentMultiLineTokenTests
{
    /// <summary>
    /// <see cref="CommentMultiLineToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="CommentMultiLineToken.TextSpan"/>
    /// <see cref="CommentMultiLineToken.SyntaxKind"/>
    /// <see cref="CommentMultiLineToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"/* Apple Sauce */";
        var targetSubstring = "/* Apple Sauce */";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var commentMultiLineToken = new CommentMultiLineToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, commentMultiLineToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.CommentMultiLineToken, commentMultiLineToken.SyntaxKind);
        Assert.False(commentMultiLineToken.IsFabricated);
    }
}