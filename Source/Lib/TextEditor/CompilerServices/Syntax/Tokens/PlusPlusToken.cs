using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct PlusPlusToken : ISyntaxToken
{
    public PlusPlusToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.PlusPlusToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}