using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="CommaToken"/>
/// </summary>
public class CommaTokenTests
{
    /// <summary>
    /// <see cref="CommaToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="CommaToken.TextSpan"/>
    /// <see cref="CommaToken.SyntaxKind"/>
    /// <see cref="CommaToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @",";
        var targetSubstring = ",";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var commaToken = new CommaToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, commaToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.CommaToken, commaToken.SyntaxKind);
        Assert.False(commaToken.IsFabricated);
    }
}