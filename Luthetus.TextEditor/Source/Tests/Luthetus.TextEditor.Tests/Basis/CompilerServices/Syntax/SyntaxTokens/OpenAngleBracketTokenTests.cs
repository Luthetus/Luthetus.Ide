using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="OpenAngleBracketToken"/>
/// </summary>
public class OpenAngleBracketTokenTests
{
    /// <summary>
    /// <see cref="OpenAngleBracketToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="OpenAngleBracketToken.TextSpan"/>
    /// <see cref="OpenAngleBracketToken.SyntaxKind"/>
    /// <see cref="OpenAngleBracketToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"<";
        var targetSubstring = "<";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var openAngleBracketToken = new OpenAngleBracketToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, openAngleBracketToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.OpenAngleBracketToken, openAngleBracketToken.SyntaxKind);
        Assert.False(openAngleBracketToken.IsFabricated);
    }
}