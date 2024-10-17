using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct AssociatedNameToken : ISyntaxToken
{
    public AssociatedNameToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.AssociatedNameToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}
