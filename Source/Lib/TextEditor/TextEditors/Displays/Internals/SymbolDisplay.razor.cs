using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class SymbolDisplay : ComponentBase, ITextEditorSymbolRenderer
{
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITextEditorSymbol Symbol { get; set; } = null!;
    
    private Task OpenInEditorOnClick(string filePath)
    {
    	return TextEditorService.OpenInEditorAsync(
			filePath,
			true,
			null,
			new Category("main"),
			Key<TextEditorViewModel>.NewKey());
    }
}