using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxTokens;

public sealed record KeywordTokenTests
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
