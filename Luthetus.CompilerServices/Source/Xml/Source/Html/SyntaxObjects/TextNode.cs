using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Xml.Html.SyntaxObjects;

public class TextNode : IHtmlSyntaxNode
{
    public TextNode(TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;

        ChildContent = ImmutableArray<IHtmlSyntax>.Empty;
        Children = ImmutableArray<IHtmlSyntax>.Empty;
    }

    public ImmutableArray<IHtmlSyntax> ChildContent { get; }
    public ImmutableArray<IHtmlSyntax> Children { get; }
    public TextEditorTextSpan TextEditorTextSpan { get; }

    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.TextNode;
}