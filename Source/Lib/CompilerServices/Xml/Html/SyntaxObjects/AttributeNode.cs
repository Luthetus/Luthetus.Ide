using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Xml.Html.SyntaxObjects;

public class AttributeNode : IHtmlSyntaxNode
{
    public AttributeNode(
        AttributeNameNode attributeNameSyntax,
        AttributeValueNode attributeValueSyntax)
    {
        AttributeNameSyntax = attributeNameSyntax;
        AttributeValueSyntax = attributeValueSyntax;

        ChildContent = new IHtmlSyntax[]
        {
        AttributeNameSyntax,
        AttributeValueSyntax,
        }.ToImmutableArray();

        Children = ChildContent;
    }

    public AttributeNameNode AttributeNameSyntax { get; }
    public AttributeValueNode AttributeValueSyntax { get; }

    public ImmutableArray<IHtmlSyntax> ChildContent { get; }
    public ImmutableArray<IHtmlSyntax> Children { get; }

    public TextEditorTextSpan TextEditorTextSpan => new(
        AttributeNameSyntax.TextEditorTextSpan.StartingIndexInclusive,
        AttributeValueSyntax.TextEditorTextSpan.EndingIndexExclusive,
        (byte)GenericDecorationKind.None,
        AttributeNameSyntax.TextEditorTextSpan.ResourceUri,
        AttributeNameSyntax.TextEditorTextSpan.SourceText);

    public HtmlSyntaxKind HtmlSyntaxKind => HtmlSyntaxKind.AttributeNode;
}