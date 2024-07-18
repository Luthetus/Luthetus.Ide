using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="EndOfFileToken"/>
/// </summary>
public class EndOfFileTokenTests
{
    /// <summary>
    /// <see cref="EndOfFileToken(TextEditorTextSpan)"/>
    /// <see cref="EndOfFileToken.TextSpan"/>
    /// <see cref="EndOfFileToken.SyntaxKind"/>
    /// <see cref="EndOfFileToken.IsFabricated"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        var text = "Hello World!";
        var targetSubstring = string.Empty;
        var indexOfTokenStartInclusive = text.Length;

        var endOfFileToken = new EndOfFileToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, endOfFileToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.EndOfFileToken, endOfFileToken.SyntaxKind);
        Assert.False(endOfFileToken.IsFabricated);
	}
}