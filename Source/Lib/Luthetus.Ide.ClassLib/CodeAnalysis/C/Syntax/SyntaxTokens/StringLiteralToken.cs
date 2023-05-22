using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;

public class StringLiteralToken : ISyntaxToken
{
    public StringLiteralToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.StringLiteralToken;
}