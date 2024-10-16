using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct QuestionMarkQuestionMarkToken : ISyntaxToken
{
    public QuestionMarkQuestionMarkToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.QuestionMarkQuestionMarkToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}