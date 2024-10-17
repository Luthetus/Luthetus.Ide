using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct MinusMinusToken : ISyntaxToken
{
    public MinusMinusToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.MinusMinusToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}