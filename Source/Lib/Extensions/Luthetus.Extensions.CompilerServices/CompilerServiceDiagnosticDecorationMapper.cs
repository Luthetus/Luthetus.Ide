using Luthetus.TextEditor.RazorLib.CompilerServices.Enums;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class CompilerServiceDiagnosticDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (CompilerServiceDiagnosticDecorationKind)decorationByte;

        return decoration switch
        {
            CompilerServiceDiagnosticDecorationKind.None => string.Empty,
            CompilerServiceDiagnosticDecorationKind.DiagnosticError => "luth_te_semantic-diagnostic-error",
            CompilerServiceDiagnosticDecorationKind.DiagnosticHint => "luth_te_semantic-diagnostic-hint",
            CompilerServiceDiagnosticDecorationKind.DiagnosticSuggestion => "luth_te_semantic-diagnostic-suggestion",
            CompilerServiceDiagnosticDecorationKind.DiagnosticWarning => "luth_te_semantic-diagnostic-warning",
            CompilerServiceDiagnosticDecorationKind.DiagnosticOther => "luth_te_semantic-diagnostic-other",
            _ => string.Empty,
        };
    }
}