using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxObjects;

namespace Luthetus.Ide.ClassLib.DotNet.CSharp;

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