using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct StatementDelimiterToken : ISyntaxToken
{
    public StatementDelimiterToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.StatementDelimiterToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}