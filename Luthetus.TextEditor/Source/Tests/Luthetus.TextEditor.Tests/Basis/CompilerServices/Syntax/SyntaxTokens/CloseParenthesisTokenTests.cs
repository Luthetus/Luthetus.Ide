using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="CloseParenthesisToken"/>
/// </summary>
public class CloseParenthesisTokenTests
{
    /// <summary>
    /// <see cref="CloseParenthesisToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="CloseParenthesisToken.TextSpan"/>
    /// <see cref="CloseParenthesisToken.SyntaxKind"/>
    /// <see cref="CloseParenthesisToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @")";
        var targetSubstring = ")";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var closeParenthesisToken = new CloseParenthesisToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, closeParenthesisToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.CloseParenthesisToken, closeParenthesisToken.SyntaxKind);
        Assert.False(closeParenthesisToken.IsFabricated);
    }
}