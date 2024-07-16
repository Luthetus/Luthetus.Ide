using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.Xml.Html;

public interface IHtmlSyntaxNode : IHtmlSyntax
{
    public ImmutableArray<IHtmlSyntax> ChildContent { get; }
    public ImmutableArray<IHtmlSyntax> Children { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }
}