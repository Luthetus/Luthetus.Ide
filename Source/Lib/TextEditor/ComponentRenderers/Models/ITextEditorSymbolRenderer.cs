using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;

public interface ITextEditorSymbolRenderer
{
    public Symbol Symbol { get; set; }
}