using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="OpenBraceToken"/>
/// </summary>
public class OpenBraceTokenTests
{
    /// <summary>
    /// <see cref="OpenBraceToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="OpenBraceToken.TextSpan"/>
    /// <see cref="OpenBraceToken.SyntaxKind"/>
    /// <see cref="OpenBraceToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"{";
        var targetSubstring = "{";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var openBraceToken = new OpenBraceToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, openBraceToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.OpenBraceToken, openBraceToken.SyntaxKind);
        Assert.False(openBraceToken.IsFabricated);
    }
}