using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="CloseAngleBracketToken"/>
/// </summary>
public class CloseAngleBracketTokenTests
{
    /// <summary>
    /// <see cref="CloseAngleBracketToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="CloseAngleBracketToken.TextSpan"/>
    /// <see cref="CloseAngleBracketToken.SyntaxKind"/>
    /// <see cref="CloseAngleBracketToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @">";
        var targetSubstring = ">";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var closeAngleBracketToken = new CloseAngleBracketToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, closeAngleBracketToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.CloseAngleBracketToken, closeAngleBracketToken.SyntaxKind);
        Assert.False(closeAngleBracketToken.IsFabricated);
    }
}