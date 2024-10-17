using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct QuestionMarkToken : ISyntaxToken
{
    public QuestionMarkToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.QuestionMarkToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}