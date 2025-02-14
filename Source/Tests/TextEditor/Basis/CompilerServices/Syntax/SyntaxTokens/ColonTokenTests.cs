using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="ColonToken"/>
/// </summary>
public class ColonTokenTests
{
    /// <summary>
    /// <see cref="ColonToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="ColonToken.TextSpan"/>
    /// <see cref="ColonToken.SyntaxKind"/>
    /// <see cref="ColonToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @":";
        var targetSubstring = ":";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var colonToken = new ColonToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, colonToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.ColonToken, colonToken.SyntaxKind);
        Assert.False(colonToken.IsFabricated);
    }
}