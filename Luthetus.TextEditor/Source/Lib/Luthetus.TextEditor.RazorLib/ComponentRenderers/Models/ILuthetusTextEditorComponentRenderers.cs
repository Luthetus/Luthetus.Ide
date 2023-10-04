namespace Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;

public interface ILuthetusTextEditorComponentRenderers
{
    public Type SymbolRendererType { get; }
    public Type DiagnosticRendererType { get; }
}