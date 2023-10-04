using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxObjects;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.Lang.Xml.Html;

public class HtmlSyntaxUnit
{
    public HtmlSyntaxUnit(
        TagNode rootTagSyntax,
        LuthetusDiagnosticBag diagnosticBag)
    {
        DiagnosticBag = diagnosticBag;
        RootTagSyntax = rootTagSyntax;
    }

    public TagNode RootTagSyntax { get; }
    public LuthetusDiagnosticBag DiagnosticBag { get; }

    public class HtmlSyntaxUnitBuilder
    {
        public HtmlSyntaxUnitBuilder(TagNode rootTagSyntax, LuthetusDiagnosticBag diagnosticBag)
        {
            RootTagSyntax = rootTagSyntax;
            DiagnosticBag = diagnosticBag;
        }

        public TagNode RootTagSyntax { get; }
        public LuthetusDiagnosticBag DiagnosticBag { get; }

        public HtmlSyntaxUnit Build()
        {
            return new HtmlSyntaxUnit(
                RootTagSyntax,
                DiagnosticBag);
        }
    }
}