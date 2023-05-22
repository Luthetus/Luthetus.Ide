using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxObjects;

namespace Luthetus.Ide.ClassLib.DotNet.CSharp;

public class CSharpProjectSyntaxWalker : XmlSyntaxWalker
{
    public List<TagSyntax> TagSyntaxes { get; } = new();

    public override void VisitTagSyntax(TagSyntax tagSyntax)
    {
        TagSyntaxes.Add(tagSyntax);
        base.VisitTagSyntax(tagSyntax);
    }
}