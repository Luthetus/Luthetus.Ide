using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.CSharp.Syntax.SyntaxTokens;

public class OpenBraceToken : ISyntaxToken
{
    public OpenBraceToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }

    public SyntaxKind SyntaxKind => SyntaxKind.OpenBraceToken;
}
