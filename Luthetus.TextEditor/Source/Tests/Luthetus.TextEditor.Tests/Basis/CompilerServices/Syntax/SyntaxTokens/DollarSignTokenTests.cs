using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="DollarSignToken"/>
/// </summary>
public class DollarSignTokenTests
{
    /// <summary>
    /// <see cref="DollarSignToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="DollarSignToken.TextSpan"/>
    /// <see cref="DollarSignToken.SyntaxKind"/>
    /// <see cref="DollarSignToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"$";
        var targetSubstring = "$";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var dollarSignToken = new DollarSignToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, dollarSignToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.DollarSignToken, dollarSignToken.SyntaxKind);
        Assert.False(dollarSignToken.IsFabricated);
    }
}