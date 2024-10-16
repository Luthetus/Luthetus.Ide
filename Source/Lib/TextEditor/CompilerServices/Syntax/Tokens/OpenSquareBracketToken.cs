using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct OpenSquareBracketToken : ISyntaxToken
{
    public OpenSquareBracketToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.OpenSquareBracketToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}