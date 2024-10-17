using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct AssociatedValueToken : ISyntaxToken
{
    public AssociatedValueToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.AssociatedValueToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}