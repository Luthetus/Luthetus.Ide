using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

/// <summary>
/// Marks the return to the 'string literal' part of an interpolated string
/// (i.e.: the end of an interpolated expression, but there might be more interpolated expressions to come).
/// </summary>
public struct StringInterpolatedContinueToken : ISyntaxToken
{
    public StringInterpolatedContinueToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.StringInterpolatedContinueToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}
