using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public sealed record CloseParenthesisToken : ISyntaxToken
{
    public CloseParenthesisToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CloseParenthesisToken;
    public bool IsFabricated { get; init; }
}