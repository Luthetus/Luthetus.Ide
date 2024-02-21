using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="MinusMinusToken"/>
/// </summary>
public sealed record MinusMinusTokenTests
{
    /// <summary>
    /// <see cref="MinusMinusToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="MinusMinusToken.TextSpan"/>
    /// <see cref="MinusMinusToken.SyntaxKind"/>
    /// <see cref="MinusMinusToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"--";
        var targetSubstring = "--";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var minusMinusToken = new MinusMinusToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, minusMinusToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.MinusMinusToken, minusMinusToken.SyntaxKind);
        Assert.False(minusMinusToken.IsFabricated);
    }
}