using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxObjects;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;

public class GenericSyntaxUnit
{
    public GenericSyntaxUnit(
        GenericDocumentSyntax genericDocumentSyntax,
        DiagnosticBag diagnosticBag)
    {
        GenericDocumentSyntax = genericDocumentSyntax;
        DiagnosticBag = diagnosticBag;
    }

    public GenericDocumentSyntax GenericDocumentSyntax { get; }
    public DiagnosticBag DiagnosticBag { get; }
}