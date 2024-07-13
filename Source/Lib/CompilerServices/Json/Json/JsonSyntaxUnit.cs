using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.CompilerServices.Lang.Json.Json.SyntaxObjects;

namespace Luthetus.CompilerServices.Lang.Json.Json;

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