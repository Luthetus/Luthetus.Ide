using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.CompilerServices.Xml.Html.SyntaxEnums;

namespace Luthetus.CompilerServices.Xml.Html.SyntaxObjects;

public class TagNameNode : IHtmlSyntaxNode
{
    public TagNameNode(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;

        ChildContent = ImmutableArray<IHtmlSyntax>.Empty;
        Children = ImmutableArray<IHtmlSyntax>.Empty;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IHtmlSyntax> ChildContent { get; }
    public ImmutableArray<IHtmlSyntax> Children { get; }

    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.TagNameNode;
}