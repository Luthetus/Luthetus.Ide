using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

/// <summary>
/// Marks the end of a 'StringInterpolatedStartToken'.
/// </summary>
public struct StringInterpolatedEndToken : ISyntaxToken
{
    public StringInterpolatedEndToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.StringInterpolatedEndToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}