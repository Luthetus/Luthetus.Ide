using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct OpenBraceToken : ISyntaxToken
{
    public OpenBraceToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.OpenBraceToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}