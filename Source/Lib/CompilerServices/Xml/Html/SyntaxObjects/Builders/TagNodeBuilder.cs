using Luthetus.CompilerServices.Xml.Html.SyntaxEnums;

namespace Luthetus.CompilerServices.Xml.Html.SyntaxObjects.Builders;

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
            AttributeSyntaxes,
            Children,
            HtmlSyntaxKind,
            HasSpecialHtmlCharacter);
    }
}