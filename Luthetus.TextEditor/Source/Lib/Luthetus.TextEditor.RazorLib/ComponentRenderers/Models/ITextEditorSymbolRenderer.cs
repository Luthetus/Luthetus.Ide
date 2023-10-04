using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;

public interface ITextEditorSymbolRenderer
{
    public ITextEditorSymbol Symbol { get; set; }
}