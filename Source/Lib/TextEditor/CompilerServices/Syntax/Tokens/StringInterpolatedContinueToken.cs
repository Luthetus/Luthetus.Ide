using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

/// <summary>
/// Used to signify a return to the 'string literal' part of an interpolated string
/// (i.e.: that an interpolated expression ended, but that the interpolated string is not terminated).
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
