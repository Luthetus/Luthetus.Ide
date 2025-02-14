using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="MemberAccessToken"/>
/// </summary>
public class MemberAccessTokenTests
{
    /// <summary>
    /// <see cref="MemberAccessToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="MemberAccessToken.TextSpan"/>
    /// <see cref="MemberAccessToken.SyntaxKind"/>
    /// <see cref="MemberAccessToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @".";
        var targetSubstring = ".";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var memberAccessToken = new MemberAccessToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, memberAccessToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.MemberAccessToken, memberAccessToken.SyntaxKind);
        Assert.False(memberAccessToken.IsFabricated);
    }
}