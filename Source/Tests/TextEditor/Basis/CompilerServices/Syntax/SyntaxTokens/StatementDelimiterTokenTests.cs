using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

/// <summary>
/// <see cref="StatementDelimiterToken"/>
/// </summary>
public class StatementDelimiterTokenTests
{
    /// <summary>
    /// <see cref="StatementDelimiterToken(TextEditorTextSpan)"/>
    /// <br/>----<br/>
    /// <see cref="StatementDelimiterToken.TextSpan"/>
    /// <see cref="StatementDelimiterToken.SyntaxKind"/>
    /// <see cref="StatementDelimiterToken.IsFabricated"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var text = @";";
        var targetSubstring = ";";
        var indexOfTokenStartInclusive = text.IndexOf(targetSubstring);

        var statementDelimiterToken = new StatementDelimiterToken(new TextEditorTextSpan(
            indexOfTokenStartInclusive,
            indexOfTokenStartInclusive + targetSubstring.Length,
            0,
            new ResourceUri("/unitTesting.txt"),
            text));

        Assert.Equal(targetSubstring, statementDelimiterToken.TextSpan.GetText());
        Assert.Equal(SyntaxKind.StatementDelimiterToken, statementDelimiterToken.SyntaxKind);
        Assert.False(statementDelimiterToken.IsFabricated);
    }
}