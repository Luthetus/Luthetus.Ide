using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.CSharp.Syntax.SyntaxTokens;

public class CloseBraceToken : ISyntaxToken
{
    public CloseBraceToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }

    public SyntaxKind SyntaxKind => SyntaxKind.CloseBraceToken;
}
