using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.CompilerServices.Xml.Html.SyntaxObjects;

namespace Luthetus.CompilerServices.Xml.Html;

public class HtmlSyntaxUnit
{
    public HtmlSyntaxUnit(
        TagNode rootTagSyntax,
        DiagnosticBag diagnosticBag)
    {
        DiagnosticBag = diagnosticBag;
        RootTagSyntax = rootTagSyntax;
    }

    public TagNode RootTagSyntax { get; }
    public DiagnosticBag DiagnosticBag { get; }

    public class HtmlSyntaxUnitBuilder
    {
        public HtmlSyntaxUnitBuilder(TagNode rootTagSyntax, DiagnosticBag diagnosticBag)
        {
            RootTagSyntax = rootTagSyntax;
            DiagnosticBag = diagnosticBag;
        }

        public TagNode RootTagSyntax { get; }
        public DiagnosticBag DiagnosticBag { get; }

        public HtmlSyntaxUnit Build()
        {
            return new HtmlSyntaxUnit(
                RootTagSyntax,
                DiagnosticBag);
        }
    }
}