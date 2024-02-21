using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="StringLiteralToken"/>
/// </summary>
public class StringLiteralTokenTests
{
    /// <summary>
    /// <see cref="StringLiteralToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="StringLiteralToken.TextSpan"/>
    /// <see cref="StringLiteralToken.SyntaxKind"/>
    /// <see cref="StringLiteralToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"Hello World!";
        var targetSubstring = "Hello World!";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var stringLiteralToken = new StringLiteralToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, stringLiteralToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.StringLiteralToken, stringLiteralToken.SyntaxKind);
        Assert.False(stringLiteralToken.IsFabricated);
    }
}