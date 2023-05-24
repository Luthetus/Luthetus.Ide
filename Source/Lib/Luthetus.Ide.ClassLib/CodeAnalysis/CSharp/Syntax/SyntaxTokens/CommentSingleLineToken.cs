using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.CSharp.Syntax.SyntaxTokens;

public class CommentSingleLineToken : ISyntaxToken
{
    public CommentSingleLineToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }

    public SyntaxKind SyntaxKind => SyntaxKind.CommentSingleLineToken;
}