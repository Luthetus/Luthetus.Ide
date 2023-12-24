using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="KeywordToken"/>
/// </summary>
public class KeywordTokenTests
{
    /// <summary>
    /// <see cref="KeywordToken(TextEditorTextSpan, SyntaxKind)"/>
    /// <br/>----<br/>
    /// <see cref="KeywordToken.TextSpan"/>
    /// <see cref="KeywordToken.SyntaxKind"/>
    /// <see cref="KeywordToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"class";
        var targetSubstring = "class";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var keywordToken = new KeywordToken(
            new TextEditorTextSpan(
                indexOfTokenStartInclusive,
                indexOfTokenStartInclusive + targetSubstring.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                text),
            SyntaxKind.ClassTokenKeyword);

        Assert.Equal(targetSubstring, keywordToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.ClassTokenKeyword, keywordToken.SyntaxKind);
        Assert.False(keywordToken.IsFabricated);
    }
}
