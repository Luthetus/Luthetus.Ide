using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.Css.SyntaxEnums;

namespace Luthetus.CompilerServices.Css;

public interface ICssSyntax
{
    public CssSyntaxKind CssSyntaxKind { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public IReadOnlyList<ICssSyntax> ChildCssSyntaxes { get; }
}