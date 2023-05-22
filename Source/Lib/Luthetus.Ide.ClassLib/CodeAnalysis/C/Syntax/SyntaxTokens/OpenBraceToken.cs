using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;

public class OpenBraceToken : ISyntaxToken
{
    public OpenBraceToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.OpenBraceToken;
}
