using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct CommentMultiLineToken : ISyntaxToken
{
    public CommentMultiLineToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CommentMultiLineToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}