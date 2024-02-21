using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="PlusPlusToken"/>
/// </summary>
public class PlusPlusTokenTests
{
    /// <summary>
    /// <see cref="PlusPlusToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="PlusPlusToken.TextSpan"/>
    /// <see cref="PlusPlusToken.SyntaxKind"/>
    /// <see cref="PlusPlusToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"++";
        var targetSubstring = "++";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var plusPlusToken = new PlusPlusToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, plusPlusToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.PlusPlusToken, plusPlusToken.SyntaxKind);
        Assert.False(plusPlusToken.IsFabricated);
    }
}