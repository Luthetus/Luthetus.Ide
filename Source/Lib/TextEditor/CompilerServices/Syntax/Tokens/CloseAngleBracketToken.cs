using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct CloseAngleBracketToken : ISyntaxToken
{
    public CloseAngleBracketToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CloseAngleBracketToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}