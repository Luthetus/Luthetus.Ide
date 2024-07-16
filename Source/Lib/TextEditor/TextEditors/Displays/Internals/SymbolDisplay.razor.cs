using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class SymbolDisplay : ComponentBase, ITextEditorSymbolRenderer
{
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;

    [Parameter, EditorRequired]
    public ITextEditorSymbol Symbol { get; set; } = null!;
    
    private async Task OpenInEditorOnClick(string filePath)
    {
        var resourceUri = new ResourceUri(filePath);

        if (TextEditorConfig.RegisterModelFunc is null)
            return;

        await TextEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
                resourceUri,
                ServiceProvider))
            .ConfigureAwait(false);

        if (TextEditorConfig.TryRegisterViewModelFunc is not null)
        {
            var viewModelKey = await TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
                    Key<TextEditorViewModel>.NewKey(),
                    resourceUri,
                    new Category("main"),
                    false,
                    ServiceProvider))
                .ConfigureAwait(false);

            if (viewModelKey != Key<TextEditorViewModel>.Empty &&
                TextEditorConfig.TryShowViewModelFunc is not null)
            {
                await TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
                        viewModelKey,
                        Key<TextEditorGroup>.Empty,
                        ServiceProvider))
                    .ConfigureAwait(false);
            }
        }
    }
}