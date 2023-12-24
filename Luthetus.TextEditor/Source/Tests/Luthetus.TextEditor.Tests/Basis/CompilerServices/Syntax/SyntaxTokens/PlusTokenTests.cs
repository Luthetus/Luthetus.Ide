using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="PlusToken"/>
/// </summary>
public class PlusTokenTests
{
    /// <summary>
    /// <see cref="PlusToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="PlusToken.TextSpan"/>
    /// <see cref="PlusToken.SyntaxKind"/>
    /// <see cref="PlusToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"+";
        var targetSubstring = "+";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var plusToken = new PlusToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, plusToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.PlusToken, plusToken.SyntaxKind);
        Assert.False(plusToken.IsFabricated);
    }
}