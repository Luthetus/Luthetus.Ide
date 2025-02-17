using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.Css.SyntaxEnums;

namespace Luthetus.CompilerServices.Css.SyntaxObjects;

public class CssDocumentSyntax : ICssSyntax
{
    public CssDocumentSyntax(
        TextEditorTextSpan textEditorTextSpan,
		IReadOnlyList<ICssSyntax> childCssSyntaxes)
    {
        ChildCssSyntaxes = childCssSyntaxes;
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public IReadOnlyList<ICssSyntax> ChildCssSyntaxes { get; }

    public CssSyntaxKind CssSyntaxKind => CssSyntaxKind.Document;
}