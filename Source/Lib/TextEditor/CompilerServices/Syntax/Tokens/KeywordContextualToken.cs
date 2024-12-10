using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct KeywordContextualToken : ISyntaxToken
{
    public KeywordContextualToken(TextEditorTextSpan textSpan, SyntaxKind syntaxKind)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
        SyntaxKind = syntaxKind;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind { get; }
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}