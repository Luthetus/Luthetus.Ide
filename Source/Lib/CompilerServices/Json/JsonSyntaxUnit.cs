using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.CompilerServices.Json.SyntaxObjects;

namespace Luthetus.CompilerServices.Json;

public class JsonSyntaxUnit
{
    public JsonSyntaxUnit(
        JsonDocumentSyntax jsonDocumentSyntax,
        DiagnosticBag diagnosticBag)
    {
        JsonDocumentSyntax = jsonDocumentSyntax;
        DiagnosticBag = diagnosticBag;
    }

    public JsonDocumentSyntax JsonDocumentSyntax { get; }
    public DiagnosticBag DiagnosticBag { get; }
}