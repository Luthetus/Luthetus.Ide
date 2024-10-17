using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct MemberAccessToken : ISyntaxToken
{
    public MemberAccessToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.MemberAccessToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}