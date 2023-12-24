using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="QuestionMarkQuestionMarkToken"/>
/// </summary>
public sealed record QuestionMarkQuestionMarkTokenTests
{
    /// <summary>
    /// <see cref="QuestionMarkQuestionMarkToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="QuestionMarkQuestionMarkToken.TextSpan"/>
    /// <see cref="QuestionMarkQuestionMarkToken.SyntaxKind"/>
    /// <see cref="QuestionMarkQuestionMarkToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @"??";
        var targetSubstring = "??";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var questionMarkQuestionMarkToken = new QuestionMarkQuestionMarkToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, questionMarkQuestionMarkToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.QuestionMarkQuestionMarkToken, questionMarkQuestionMarkToken.SyntaxKind);
        Assert.False(questionMarkQuestionMarkToken.IsFabricated);
    }
}