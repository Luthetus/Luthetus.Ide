using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct NumericLiteralToken : ISyntaxToken
{
    public NumericLiteralToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.NumericLiteralToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}
