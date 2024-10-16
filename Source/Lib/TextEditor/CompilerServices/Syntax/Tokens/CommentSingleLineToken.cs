using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct CommentSingleLineToken : ISyntaxToken
{
    public CommentSingleLineToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CommentSingleLineToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}