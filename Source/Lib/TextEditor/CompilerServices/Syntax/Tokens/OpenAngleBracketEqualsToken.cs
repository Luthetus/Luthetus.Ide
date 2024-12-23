using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct OpenAngleBracketEqualsToken : ISyntaxToken
{
    public OpenAngleBracketEqualsToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.OpenAngleBracketEqualsToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}
