using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;

public interface ITextEditorDiagnosticRenderer
{
    public TextEditorDiagnostic Diagnostic { get; set; }
}