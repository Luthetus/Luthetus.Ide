using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Xml.Html.SyntaxObjects;

public class AttributeNameNode : IHtmlSyntaxNode
{
    public AttributeNameNode(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextEditorTextSpan = textEditorTextSpan;

        ChildContent = ImmutableArray<IHtmlSyntax>.Empty;
        Children = ImmutableArray<IHtmlSyntax>.Empty;
    }

    public TextEditorTextSpan TextEditorTextSpan { get; }
    public ImmutableArray<IHtmlSyntax> ChildContent { get; }
    public ImmutableArray<IHtmlSyntax> Children { get; }

    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.AttributeNameNode;
}