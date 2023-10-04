using Luthetus.CompilerServices.Lang.Css.Css.SyntaxObjects;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.Lang.Css.Css;

public class CssSyntaxUnit
{
    public CssSyntaxUnit(
        CssDocumentSyntax cssDocumentSyntax,
        LuthetusDiagnosticBag diagnosticBag)
    {
        CssDocumentSyntax = cssDocumentSyntax;
        this.diagnosticBag = diagnosticBag;
    }

    public CssDocumentSyntax CssDocumentSyntax { get; }
    public LuthetusDiagnosticBag diagnosticBag { get; }
}