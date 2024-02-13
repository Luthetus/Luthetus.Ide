using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="CommentSingleLineToken"/>
/// </summary>
public class CommentSingleLineTokenTests
{
    /// <summary>
    /// <see cref="CommentSingleLineToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="CommentSingleLineToken.TextSpan"/>
    /// <see cref="CommentSingleLineToken.SyntaxKind"/>
    /// <see cref="CommentSingleLineToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"// Alphabet Soup";
        var targetSubstring = "// Alphabet Soup";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var commentSingleLineToken = new CommentSingleLineToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, commentSingleLineToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.CommentSingleLineToken, commentSingleLineToken.SyntaxKind);
        Assert.False(commentSingleLineToken.IsFabricated);
    }
}