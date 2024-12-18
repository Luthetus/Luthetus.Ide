using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct AmpersandAmpersandToken : ISyntaxToken
{
    public AmpersandAmpersandToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.AmpersandAmpersandToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}
