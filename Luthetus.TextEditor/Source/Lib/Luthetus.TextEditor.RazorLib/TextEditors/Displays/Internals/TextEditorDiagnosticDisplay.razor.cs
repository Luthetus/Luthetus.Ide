using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorDiagnosticDisplay : ComponentBase, ITextEditorDiagnosticRenderer
{
    [Parameter, EditorRequired]
    public TextEditorDiagnostic Diagnostic { get; set; } = null!;
}