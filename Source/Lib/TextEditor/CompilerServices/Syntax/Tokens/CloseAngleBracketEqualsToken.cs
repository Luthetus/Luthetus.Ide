using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct CloseAngleBracketEqualsToken : ISyntaxToken
{
    public CloseAngleBracketEqualsToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.CloseAngleBracketEqualsToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}
