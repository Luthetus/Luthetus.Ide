using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

/// <summary>
/// Making types related to JSON key-value pairs.
/// Group is for when the value is a grouping of multiple values
/// </summary>
public struct OpenAssociatedGroupToken : ISyntaxToken
{
    public OpenAssociatedGroupToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.OpenAssociatedGroupToken;
    public bool ConstructorWasInvoked { get; }
}
