using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

public sealed record NumericLiteralTokenTests
{
    public NumericLiteralToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.NumericLiteralToken;
    public bool IsFabricated { get; init; }
}