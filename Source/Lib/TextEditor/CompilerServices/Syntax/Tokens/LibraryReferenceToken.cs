using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public struct LibraryReferenceToken : ISyntaxToken
{
    public LibraryReferenceToken(TextEditorTextSpan textSpan, bool isAbsolutePath)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
        IsAbsolutePath = isAbsolutePath;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.LibraryReferenceToken;
    public bool IsAbsolutePath { get; }
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}