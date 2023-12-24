using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="QuestionMarkToken"/>
/// </summary>
public class QuestionMarkTokenTests
{
    /// <summary>
    /// <see cref="QuestionMarkToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="QuestionMarkToken.TextSpan"/>
    /// <see cref="QuestionMarkToken.SyntaxKind"/>
    /// <see cref="QuestionMarkToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"?";
        var targetSubstring = "?";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var questionMarkToken = new QuestionMarkToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, questionMarkToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.QuestionMarkToken, questionMarkToken.SyntaxKind);
        Assert.False(questionMarkToken.IsFabricated);
    }
}