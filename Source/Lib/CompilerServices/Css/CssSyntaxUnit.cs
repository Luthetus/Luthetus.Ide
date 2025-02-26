using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.CompilerServices.Css.SyntaxObjects;

namespace Luthetus.CompilerServices.Css;

public class CssSyntaxUnit
{
    public CssSyntaxUnit(
        CssDocumentSyntax cssDocumentSyntax,
        List<TextEditorDiagnostic> diagnosticList)
    {
        CssDocumentSyntax = cssDocumentSyntax;
        DiagnosticList = diagnosticList;
    }

    public CssDocumentSyntax CssDocumentSyntax { get; }
    public List<TextEditorDiagnostic> DiagnosticList { get; }
}