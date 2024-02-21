using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="CloseBraceToken"/>
/// </summary>
public class CloseBraceTokenTests
{
    /// <summary>
    /// <see cref="CloseBraceToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="CloseBraceToken.TextSpan"/>
    /// <see cref="CloseBraceToken.SyntaxKind"/>
    /// <see cref="CloseBraceToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"}";
        var targetSubstring = "}";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var closeBraceToken = new CloseBraceToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, closeBraceToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.CloseBraceToken, closeBraceToken.SyntaxKind);
        Assert.False(closeBraceToken.IsFabricated);
    }
}