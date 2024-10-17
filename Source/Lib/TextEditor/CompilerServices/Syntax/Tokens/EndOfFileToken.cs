using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct EndOfFileToken : ISyntaxToken
{
    public EndOfFileToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.EndOfFileToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}