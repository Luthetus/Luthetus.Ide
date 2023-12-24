using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="MinusToken"/>
/// </summary>
public class MinusTokenTests
{
    /// <summary>
    /// <see cref="MinusToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="MinusToken.TextSpan"/>
    /// <see cref="MinusToken.SyntaxKind"/>
    /// <see cref="MinusToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"-";
        var targetSubstring = "-";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var minusToken = new MinusToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, minusToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.MinusToken, minusToken.SyntaxKind);
        Assert.False(minusToken.IsFabricated);
    }
}