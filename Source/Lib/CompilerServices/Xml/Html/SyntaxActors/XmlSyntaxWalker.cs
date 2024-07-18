using Luthetus.CompilerServices.Xml.Html.SyntaxEnums;
using Luthetus.CompilerServices.Xml.Html.SyntaxObjects;

namespace Luthetus.CompilerServices.Xml.Html.SyntaxActors;

public abstract class XmlSyntaxWalker
{
    public virtual void Visit(IHtmlSyntaxNode node)
    {
        foreach (var child in node.Children)
        {
            if (child is null)
                continue;

            if (child.HtmlSyntaxKind.ToString().EndsWith("Node"))
                Visit((IHtmlSyntaxNode)child);
        }

        switch (node.HtmlSyntaxKind)
        {
            case HtmlSyntaxKind.AttributeNameNode:
                VisitAttributeNameNode((AttributeNameNode)node);
                break;
            case HtmlSyntaxKind.AttributeValueNode:
                VisitAttributeValueNode((AttributeValueNode)node);
                break;
            case HtmlSyntaxKind.AttributeNode:
                VisitAttributeNode((AttributeNode)node);
                break;
            case HtmlSyntaxKind.CommentNode:
                VisitCommentNode((CommentNode)node);
                break;
            case HtmlSyntaxKind.InjectedLanguageFragmentNode:
                VisitInjectedLanguageFragmentNode((InjectedLanguageFragmentNode)node);
                break;
            case HtmlSyntaxKind.TagNameNode:
                VisitTagNameNode((TagNameNode)node);
                break;
            case HtmlSyntaxKind.TextNode:
                VisitTextNode((TextNode)node);
                break;
            case HtmlSyntaxKind.TagOpeningNode:
                VisitTagOpeningNode((TagNode)node);
                break;
            case HtmlSyntaxKind.TagClosingNode:
                VisitTagClosingNode((TagNode)node);
                break;
            case HtmlSyntaxKind.TagSelfClosingNode:
                VisitTagSelfClosingNode((TagNode)node);
                break;
        }
    }

    public virtual void VisitAttributeNameNode(AttributeNameNode node)
    {

    }

    public virtual void VisitAttributeValueNode(AttributeValueNode node)
    {

    }

    public virtual void VisitAttributeNode(AttributeNode node)
    {

    }

    public virtual void VisitCommentNode(CommentNode node)
    {

    }

    public virtual void VisitInjectedLanguageFragmentNode(InjectedLanguageFragmentNode node)
    {

    }

    public virtual void VisitTagNameNode(TagNameNode node)
    {

    }

    public virtual void VisitTextNode(TextNode node)
    {

    }

    public virtual void VisitTagOpeningNode(TagNode node)
    {

    }

    public virtual void VisitTagClosingNode(TagNode node)
    {

    }

    public virtual void VisitTagSelfClosingNode(TagNode node)
    {

    }
}