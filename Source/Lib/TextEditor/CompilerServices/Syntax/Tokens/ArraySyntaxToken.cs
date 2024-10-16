using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct ArraySyntaxToken : ISyntaxToken
{
    public ArraySyntaxToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.ArraySyntaxToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}