using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="EqualsToken"/>
/// </summary>
public class EqualsTokenTests
{
    /// <summary>
    /// <see cref="EqualsToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="EqualsToken.TextSpan"/>
    /// <see cref="EqualsToken.SyntaxKind"/>
    /// <see cref="EqualsToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"=";
        var targetSubstring = "=";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var equalsToken = new EqualsToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, equalsToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.EqualsToken, equalsToken.SyntaxKind);
        Assert.False(equalsToken.IsFabricated);
    }
}