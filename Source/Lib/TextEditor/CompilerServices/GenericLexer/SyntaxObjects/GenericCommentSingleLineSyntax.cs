using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxObjects;

public class GenericCommentSingleLineSyntax : IGenericSyntax
{
    public GenericCommentSingleLineSyntax(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public IReadOnlyList<IGenericSyntax> ChildList => Array.Empty<IGenericSyntax>();
    public GenericSyntaxKind GenericSyntaxKind => GenericSyntaxKind.CommentSingleLine;
}