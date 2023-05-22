using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;

public class LibraryReferenceToken : ISyntaxToken
{
    public LibraryReferenceToken(
        TextEditorTextSpan textEditorTextSpan,
        bool isAbsolutePath)
    {
        TextEditorTextSpan = textEditorTextSpan;
        IsAbsolutePath = isAbsolutePath;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.LibraryReferenceToken;
    public bool IsAbsolutePath { get; }
}