using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="IdentifierToken"/>
/// </summary>
public class IdentifierTokenTests
{
    /// <summary>
    /// <see cref="IdentifierToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="IdentifierToken.TextSpan"/>
    /// <see cref="IdentifierToken.SyntaxKind"/>
    /// <see cref="IdentifierToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"someVariable";
        var targetSubstring = "someVariable";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var identifierToken = new IdentifierToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, identifierToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.IdentifierToken, identifierToken.SyntaxKind);
        Assert.False(identifierToken.IsFabricated);
    }
}