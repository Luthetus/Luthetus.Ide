using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

public struct SyntaxToken : ISyntax
{
    public SyntaxToken(SyntaxKind syntaxKind, TextEditorTextSpan textSpan)
    {
        SyntaxKind = syntaxKind;
        TextSpan = textSpan;
    }

    public SyntaxKind SyntaxKind { get; }
    
    /// <summary>TODO: Remove the setter.</summary>
    public TextEditorTextSpan TextSpan { get; set; }
    
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked => TextSpan.ConstructorWasInvoked;
}
