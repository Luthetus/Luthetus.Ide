using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxActors;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxObjects;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;

public class CSharpProjectSyntaxWalker : XmlSyntaxWalker
{
    public List<TagNode> TagNodes { get; } = new();

    public override void VisitTagOpeningNode(TagNode node)
    {
        TagNodes.Add(node);
    }

    public override void VisitTagClosingNode(TagNode node)
    {
        TagNodes.Add(node);
    }

    public override void VisitTagSelfClosingNode(TagNode node)
    {
        TagNodes.Add(node);
    }
}