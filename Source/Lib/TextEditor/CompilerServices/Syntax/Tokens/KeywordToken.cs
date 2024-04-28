using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public sealed record KeywordToken : ISyntaxToken
{
    public KeywordToken(TextEditorTextSpan textSpan, SyntaxKind syntaxKind)
    {
        TextSpan = textSpan;
        SyntaxKind = syntaxKind;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind { get; }
    public bool IsFabricated { get; init; }
}
