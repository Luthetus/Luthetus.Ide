using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="OpenSquareBracketToken"/>
/// </summary>
public class OpenSquareBracketTokenTests
{
    /// <summary>
    /// <see cref="OpenSquareBracketToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="OpenSquareBracketToken.TextSpan"/>
    /// <see cref="OpenSquareBracketToken.SyntaxKind"/>
    /// <see cref="OpenSquareBracketToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"[";
        var targetSubstring = "[";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var openSquareBracketToken = new OpenSquareBracketToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, openSquareBracketToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.OpenSquareBracketToken, openSquareBracketToken.SyntaxKind);
        Assert.False(openSquareBracketToken.IsFabricated);
    }
}