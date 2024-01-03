using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="EqualsEqualsToken"/>
/// </summary>
public class EqualsEqualsTokenTests
{
    /// <summary>
    /// <see cref="EqualsEqualsToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="EqualsEqualsToken.TextSpan"/>
    /// <see cref="EqualsEqualsToken.SyntaxKind"/>
    /// <see cref="EqualsEqualsToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"==";
        var targetSubstring = "==";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var equalsEqualsToken = new EqualsEqualsToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, equalsEqualsToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.EqualsEqualsToken, equalsEqualsToken.SyntaxKind);
        Assert.False(equalsEqualsToken.IsFabricated);
    }
}
