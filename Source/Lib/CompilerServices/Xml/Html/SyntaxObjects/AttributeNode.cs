using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.CompilerServices.Xml.Html.SyntaxEnums;

namespace Luthetus.CompilerServices.Xml.Html.SyntaxObjects;

public class AttributeNode : IHtmlSyntaxNode
{
    public AttributeNode(
        AttributeNameNode attributeNameSyntax,
        AttributeValueNode attributeValueSyntax)
    {
        AttributeNameSyntax = attributeNameSyntax;
        AttributeValueSyntax = attributeValueSyntax;

        ChildContent = new List<IHtmlSyntax>
        {
            AttributeNameSyntax,
            AttributeValueSyntax,
        };

        Children = ChildContent;
    }

    public AttributeNameNode AttributeNameSyntax { get; }
    public AttributeValueNode AttributeValueSyntax { get; }

    public IReadOnlyList<IHtmlSyntax> ChildContent { get; }
    public IReadOnlyList<IHtmlSyntax> Children { get; }

    public TextEditorTextSpan TextEditorTextSpan => new(
        AttributeNameSyntax.TextEditorTextSpan.StartInclusiveIndex,
        AttributeValueSyntax.TextEditorTextSpan.EndExclusiveIndex,
        (byte)GenericDecorationKind.None,
        AttributeNameSyntax.TextEditorTextSpan.ResourceUri,
        AttributeNameSyntax.TextEditorTextSpan.SourceText);

    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.AttributeNode;
}