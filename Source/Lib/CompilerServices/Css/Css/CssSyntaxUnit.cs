using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.CompilerServices.Css.Css.SyntaxObjects;

namespace Luthetus.CompilerServices.Css.Css;

public class CssSyntaxUnit
{
    public CssSyntaxUnit(
        CssDocumentSyntax cssDocumentSyntax,
        DiagnosticBag diagnosticBag)
    {
        CssDocumentSyntax = cssDocumentSyntax;
        this.diagnosticBag = diagnosticBag;
    }

    public CssDocumentSyntax CssDocumentSyntax { get; }
    /// <summary>
    /// TODO: Why is this named "diagnosticBag" and in the constructor "this.diagnosticBag is used"...
    ///       ...(am seeing this on 2024-07-12, I presume about a year after it was written).
    /// </summary> 
    public DiagnosticBag diagnosticBag { get; }
}