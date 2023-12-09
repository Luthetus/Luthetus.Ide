namespace Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;

public class LuthetusTextEditorComponentRenderersTests
{
    public LuthetusTextEditorComponentRenderers(Type symbolRendererType, Type diagnosticRendererType)
    {
        SymbolRendererType = symbolRendererType;
        DiagnosticRendererType = diagnosticRendererType;
    }

    public Type SymbolRendererType { get; }
    public Type DiagnosticRendererType { get; }
}