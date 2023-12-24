using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="NumericLiteralToken"/>
/// </summary>
public sealed record NumericLiteralTokenTests
{
    /// <summary>
    /// <see cref="NumericLiteralToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="NumericLiteralToken.TextSpan"/>
    /// <see cref="NumericLiteralToken.SyntaxKind"/>
    /// <see cref="NumericLiteralToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"123";
        var targetSubstring = "123";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var numericLiteralToken = new NumericLiteralToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, numericLiteralToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.NumericLiteralToken, numericLiteralToken.SyntaxKind);
        Assert.False(numericLiteralToken.IsFabricated);
    }
}