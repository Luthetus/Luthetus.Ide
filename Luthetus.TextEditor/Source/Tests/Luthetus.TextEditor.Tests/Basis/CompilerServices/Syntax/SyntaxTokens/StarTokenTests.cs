using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="StarToken"/>
/// </summary>
public class StarTokenTests
{
    /// <summary>
    /// <see cref="StarToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="StarToken.TextSpan"/>
    /// <see cref="StarToken.SyntaxKind"/>
    /// <see cref="StarToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"*";
        var targetSubstring = "*";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var starToken = new StarToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, starToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.StarToken, starToken.SyntaxKind);
        Assert.False(starToken.IsFabricated);
    }
}
