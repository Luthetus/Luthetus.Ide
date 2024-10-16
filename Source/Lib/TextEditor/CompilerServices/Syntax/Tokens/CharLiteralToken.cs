using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct CharLiteralToken : ISyntaxToken
{
    public CharLiteralToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CharLiteralToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}