using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct OpenParenthesisToken : ISyntaxToken
{
    public OpenParenthesisToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.OpenParenthesisToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}