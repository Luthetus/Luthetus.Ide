using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="KeywordContextualToken"/>
/// </summary>
public class KeywordContextualTokenTests
{
    /// <summary>
    /// <see cref="KeywordContextualToken(TextEditorTextSpan, SyntaxKind)"/>
    /// <br/>----<br/>
    /// <see cref="KeywordContextualToken.TextSpan"/>
    /// <see cref="KeywordContextualToken.SyntaxKind"/>
    /// <see cref="KeywordContextualToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"var";
        var targetSubstring = "var";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var keywordContextualToken = new KeywordContextualToken(
            new TextEditorTextSpan(
                indexOfTokenStartInclusive,
                indexOfTokenStartInclusive + targetSubstring.Length,
                0,
                new ResourceUri("/unitTesting.txt"),
                text),
            SyntaxKind.VarTokenContextualKeyword);

        Assert.Equal(targetSubstring, keywordContextualToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.VarTokenContextualKeyword, keywordContextualToken.SyntaxKind);
        Assert.False(keywordContextualToken.IsFabricated);
    }
}