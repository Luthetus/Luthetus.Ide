using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.CompilerServices.Lang.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase;

/// <summary>
/// <see cref="ExpressionDelimiter"/>
/// </summary>
public class ExpressionDelimiterTests
{
    /// <summary>
    /// <see cref="ExpressionDelimiter(SyntaxKind?, SyntaxKind, ISyntaxToken?, ISyntaxToken?)"/>
    /// <br/>----<br/>
    /// <see cref="ExpressionDelimiter.OpenSyntaxKind"/>
    /// <see cref="ExpressionDelimiter.CloseSyntaxKind"/>
    /// <see cref="ExpressionDelimiter.OpenSyntaxToken"/>
    /// <see cref="ExpressionDelimiter.CloseSyntaxToken"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var openSyntaxKind = SyntaxKind.OpenParenthesisToken;
        var closeSyntaxKind = SyntaxKind.CloseParenthesisToken;

        var openParenthesisToken = new OpenParenthesisToken(TextEditorTextSpan.FabricateTextSpan("("));
        var closeParenthesisToken = new OpenParenthesisToken(TextEditorTextSpan.FabricateTextSpan(")"));

        var expressionDelimiter = new ExpressionDelimiter(
            openSyntaxKind,
            closeSyntaxKind,
            openParenthesisToken,
            closeParenthesisToken);

        Assert.Equal(openSyntaxKind, expressionDelimiter.OpenSyntaxKind);
        Assert.Equal(closeSyntaxKind, expressionDelimiter.CloseSyntaxKind);
        Assert.Equal(openParenthesisToken, expressionDelimiter.OpenSyntaxToken);
        Assert.Equal(closeParenthesisToken, expressionDelimiter.CloseSyntaxToken);
    }
}
