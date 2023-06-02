using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;

public sealed record TriviaToken : ISyntaxToken
{
    public TriviaToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.TriviaToken;
}