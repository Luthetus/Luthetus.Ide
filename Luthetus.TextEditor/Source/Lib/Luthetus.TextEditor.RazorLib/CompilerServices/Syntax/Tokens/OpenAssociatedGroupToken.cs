using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

/// <summary>
/// Making types related to JSON key-value pairs.
/// Group is for when the value is a grouping of multiple values
/// </summary>
public sealed record OpenAssociatedGroupToken : ISyntaxToken
{
    public OpenAssociatedGroupToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.OpenAssociatedGroupToken;
}
