using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="CloseSquareBracketToken"/>
/// </summary>
public class CloseSquareBracketTokenTests
{
    /// <summary>
    /// <see cref="CloseSquareBracketToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="CloseSquareBracketToken.TextSpan"/>
    /// <see cref="CloseSquareBracketToken.SyntaxKind"/>
    /// <see cref="CloseSquareBracketToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"]";
        var targetSubstring = "]";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var closeSquareBracketToken = new CloseSquareBracketToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, closeSquareBracketToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.CloseSquareBracketToken, closeSquareBracketToken.SyntaxKind);
        Assert.False(closeSquareBracketToken.IsFabricated);
    }
}