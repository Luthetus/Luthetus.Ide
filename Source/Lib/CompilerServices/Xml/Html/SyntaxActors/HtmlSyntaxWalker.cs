using Luthetus.CompilerServices.Xml.Html.SyntaxObjects;

namespace Luthetus.CompilerServices.Xml.Html.SyntaxActors;

public class HtmlSyntaxWalker : XmlSyntaxWalker
{
    public List<AttributeNameNode> AttributeNameNodes { get; } = new();
    public List<AttributeValueNode> AttributeValueNodes { get; } = new();
    public List<InjectedLanguageFragmentNode> InjectedLanguageFragmentNodes { get; } = new();
    public List<TagNameNode> TagNameNodes { get; } = new();
    public List<CommentNode> CommentNodes { get; } = new();
    public List<TagNode> TagOpeningNodes { get; } = new();
    public List<TagNode> TagClosingNodes { get; } = new();
    public List<TagNode> TagSelfClosingNodes { get; } = new();

    public override void VisitAttributeNameNode(AttributeNameNode node)
    {
        AttributeNameNodes.Add(node);
    }

    public override void VisitAttributeValueNode(AttributeValueNode node)
    {
        AttributeValueNodes.Add(node);
    }

    public override void VisitInjectedLanguageFragmentNode(InjectedLanguageFragmentNode node)
    {
        InjectedLanguageFragmentNodes.Add(node);
    }

    public override void VisitTagNameNode(TagNameNode node)
    {
        TagNameNodes.Add(node);
    }

    public override void VisitCommentNode(CommentNode node)
    {
        CommentNodes.Add(node);
    }

    public override void VisitTagOpeningNode(TagNode node)
    {
        TagOpeningNodes.Add(node);
    }

    public override void VisitTagClosingNode(TagNode node)
    {
        TagClosingNodes.Add(node);
    }

    public override void VisitTagSelfClosingNode(TagNode node)
    {
        TagSelfClosingNodes.Add(node);
    }
}