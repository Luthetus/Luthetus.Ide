using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="OpenParenthesisToken"/>
/// </summary>
public class OpenParenthesisTokenTests
{
    /// <summary>
    /// <see cref="OpenParenthesisToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="OpenParenthesisToken.TextSpan"/>
    /// <see cref="OpenParenthesisToken.SyntaxKind"/>
    /// <see cref="OpenParenthesisToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"(";
        var targetSubstring = "(";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var openParenthesisToken = new OpenParenthesisToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, openParenthesisToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.OpenParenthesisToken, openParenthesisToken.SyntaxKind);
        Assert.False(openParenthesisToken.IsFabricated);
    }
}