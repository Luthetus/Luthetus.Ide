using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;

public class CommentSingleLineToken : ISyntaxToken
{
    public CommentSingleLineToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CommentSingleLineToken;
}