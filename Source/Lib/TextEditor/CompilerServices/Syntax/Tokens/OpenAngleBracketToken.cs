using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct OpenAngleBracketToken : ISyntaxToken
{
    public OpenAngleBracketToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.OpenAngleBracketToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}