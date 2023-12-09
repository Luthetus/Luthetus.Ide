using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

public sealed record CommentMultiLineTokenTests
{
    public CommentMultiLineToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CommentMultiLineToken;
    public bool IsFabricated { get; init; }
}