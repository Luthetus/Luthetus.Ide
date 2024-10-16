using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct CloseParenthesisToken : ISyntaxToken
{
    public CloseParenthesisToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CloseParenthesisToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}