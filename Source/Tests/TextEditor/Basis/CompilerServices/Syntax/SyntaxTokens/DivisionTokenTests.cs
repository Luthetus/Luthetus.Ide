using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="DivisionToken"/>
/// </summary>
public class DivisionTokenTests
{
    /// <summary>
    /// <see cref="DivisionToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="DivisionToken.TextSpan"/>
    /// <see cref="DivisionToken.SyntaxKind"/>
    /// <see cref="DivisionToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"/";
        var targetSubstring = "/";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var divisionToken = new DivisionToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, divisionToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.DivisionToken, divisionToken.SyntaxKind);
        Assert.False(divisionToken.IsFabricated);
    }
}