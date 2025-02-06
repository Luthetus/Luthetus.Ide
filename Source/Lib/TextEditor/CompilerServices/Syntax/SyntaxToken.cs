using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public struct SyntaxToken : ISyntaxToken
{
    public SyntaxToken(SyntaxKind syntaxKind, TextEditorTextSpan textSpan)
    {
        SyntaxKind = syntaxKind;
        TextSpan = textSpan;
    }

    public SyntaxKind SyntaxKind { get; }
    public TextEditorTextSpan TextSpan { get; }
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked => TextSpan.ConstructorWasInvoked;
}
