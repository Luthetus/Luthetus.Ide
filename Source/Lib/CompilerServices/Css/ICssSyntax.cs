using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Css.SyntaxEnums;

namespace Luthetus.CompilerServices.Css;

public interface ICssSyntax
{
    public CssSyntaxKind CssSyntaxKind { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<ICssSyntax> ChildCssSyntaxes { get; }
}