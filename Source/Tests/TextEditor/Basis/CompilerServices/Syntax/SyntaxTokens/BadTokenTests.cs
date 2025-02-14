using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="BadToken"/>
/// </summary>
public sealed record BadTokenTests
{
    /// <summary>
    /// <see cref="BadToken(TextEditorTextSpan)"/>
	/// <br/>----<br/>
    /// <see cref="BadToken.TextSpan"/>
    /// <see cref="BadToken.SyntaxKind"/>
    /// <see cref="BadToken.IsFabricated"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var text = string.Empty;
        var targetSubstring = string.Empty;
        var indexOfTokenStartInclusive = 0;

        var badToken = new BadToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, badToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.BadToken, badToken.SyntaxKind);
        Assert.False(badToken.IsFabricated);
	}
}