namespace Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;

public class LuthetusTextEditorComponentRenderers : ILuthetusTextEditorComponentRenderers
{
    public LuthetusTextEditorComponentRenderers(Type diagnosticRendererType)
    {
        DiagnosticRendererType = diagnosticRendererType;
    }

    public Type DiagnosticRendererType { get; }
}