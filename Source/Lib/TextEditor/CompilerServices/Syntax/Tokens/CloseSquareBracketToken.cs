using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct CloseSquareBracketToken : ISyntaxToken
{
    public CloseSquareBracketToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CloseSquareBracketToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}