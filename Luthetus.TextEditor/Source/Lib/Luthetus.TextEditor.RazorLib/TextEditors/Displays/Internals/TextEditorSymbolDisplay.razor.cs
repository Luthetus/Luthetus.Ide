using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextEditorSymbolDisplay : ComponentBase, ITextEditorSymbolRenderer
{
    [Parameter, EditorRequired]
    public ITextEditorSymbol Symbol { get; set; } = null!;
}