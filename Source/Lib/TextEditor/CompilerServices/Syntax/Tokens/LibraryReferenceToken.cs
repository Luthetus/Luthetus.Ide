using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

public sealed record LibraryReferenceToken : ISyntaxToken
{
    public LibraryReferenceToken(TextEditorTextSpan textSpan, bool isAbsolutePath)
    {
        TextSpan = textSpan;
        IsAbsolutePath = isAbsolutePath;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.LibraryReferenceToken;
    public bool IsAbsolutePath { get; }
    public bool IsFabricated { get; init; }
}