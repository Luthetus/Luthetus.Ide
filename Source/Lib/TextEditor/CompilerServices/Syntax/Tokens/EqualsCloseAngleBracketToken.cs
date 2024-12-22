using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct EqualsCloseAngleBracketToken : ISyntaxToken
{
    public EqualsCloseAngleBracketToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.EqualsCloseAngleBracketToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}

