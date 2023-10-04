using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxEnums;

namespace Luthetus.CompilerServices.Lang.Xml.Html.SyntaxObjects.Builders;

public class TagNodeBuilder
{
    public TagNameNode? OpenTagNameSyntax { get; set; }
    public TagNameNode? CloseTagNameSyntax { get; set; }
    public List<AttributeNode> AttributeSyntaxes { get; set; } = new();
    public List<IHtmlSyntax> Children { get; set; } = new();
    public HtmlSyntaxKind HtmlSyntaxKind { get; set; } = HtmlSyntaxKind.TagSelfClosingNode;
    public bool HasSpecialHtmlCharacter { get; set; }

    public TagNode Build()
    {
        return new TagNode(
            OpenTagNameSyntax,
            CloseTagNameSyntax,
            AttributeSyntaxes.ToImmutableArray(),
            Children.ToImmutableArray(),
            HtmlSyntaxKind,
            HasSpecialHtmlCharacter);
    }
}