using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.Css.SyntaxEnums;

namespace Luthetus.CompilerServices.Css.SyntaxObjects;

public class CssPropertyNameSyntax : ICssSyntax
{
    public CssPropertyNameSyntax(
        TextEditorTextSpan textEditorTextSpan,
        ImmutableArray<ICssSyntax> childCssSyntaxes)
    {
        ChildCssSyntaxes = childCssSyntaxes;
        TextEditorTextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<ICssSyntax> ChildCssSyntaxes { get; }

    public CssSyntaxKind CssSyntaxKind => CssSyntaxKind.PropertyName;
}