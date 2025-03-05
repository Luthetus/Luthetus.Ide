using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxObjects;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;

public class GenericSyntaxUnit
{
    public GenericSyntaxUnit(
        GenericDocumentSyntax genericDocumentSyntax,
        List<TextEditorDiagnostic> diagnosticList)
    {
        GenericDocumentSyntax = genericDocumentSyntax;
        DiagnosticList = diagnosticList;
    }

    public GenericDocumentSyntax GenericDocumentSyntax { get; }
    public IReadOnlyList<TextEditorDiagnostic> DiagnosticList { get; }
}