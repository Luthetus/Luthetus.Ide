using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.CompilerServices.Json.SyntaxObjects;

namespace Luthetus.CompilerServices.Json;

public class JsonSyntaxUnit
{
    public JsonSyntaxUnit(
        JsonDocumentSyntax jsonDocumentSyntax,
        List<TextEditorDiagnostic> diagnosticList)
    {
        JsonDocumentSyntax = jsonDocumentSyntax;
        DiagnosticList = diagnosticList;
    }

    public JsonDocumentSyntax JsonDocumentSyntax { get; }
    public List<TextEditorDiagnostic> DiagnosticList { get; }
}