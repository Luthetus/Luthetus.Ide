using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct StringLiteralToken : ISyntaxToken
{
    public StringLiteralToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.StringLiteralToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}