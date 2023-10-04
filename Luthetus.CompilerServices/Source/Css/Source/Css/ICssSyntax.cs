using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.Css.Css.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Css.Css;

public interface ICssSyntax
{
    public CssSyntaxKind CssSyntaxKind { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<ICssSyntax> ChildCssSyntaxes { get; }
}