using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;

public class TriviaToken : ISyntaxToken
{
    public TriviaToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.TriviaToken;
}