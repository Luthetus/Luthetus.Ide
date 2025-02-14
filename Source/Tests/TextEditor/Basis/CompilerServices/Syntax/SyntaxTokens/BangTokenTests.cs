using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="BangToken"/>
/// </summary>
public class BangTokenTests
{
    /// <summary>
    /// <see cref="BangToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="BangToken.TextSpan"/>
    /// <see cref="BangToken.SyntaxKind"/>
    /// <see cref="BangToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"!";
        var targetSubstring = "!";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var bangSyntaxToken = new BangToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, bangSyntaxToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.BangToken, bangSyntaxToken.SyntaxKind);
        Assert.False(bangSyntaxToken.IsFabricated);
    }
}