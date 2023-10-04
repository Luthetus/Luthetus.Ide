using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Xml.Html;

public interface IHtmlSyntaxNode : IHtmlSyntax
{
    public ImmutableArray<IHtmlSyntax> ChildContent { get; }
    public ImmutableArray<IHtmlSyntax> Children { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
}