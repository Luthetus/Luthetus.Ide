using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;

public class LibraryReferenceToken : ISyntaxToken
{
    public LibraryReferenceToken(
        TextEditorTextSpan textEditorTextSpan,
        bool isAbsolutePath)
    {
        TextSpan = textEditorTextSpan;
        IsAbsolutePath = isAbsolutePath;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.LibraryReferenceToken;
    public bool IsAbsolutePath { get; }
}