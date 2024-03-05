using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Edits.States;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.Edits.Displays;

public partial class DirtyResourceUriViewDisplay : FluxorComponent
{
    [Inject]
    private IState<DirtyResourceUriState> DirtyResourceUriStateWrap { get; set; } = null!;
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;

    private async Task OpenInEditorOnClick(string filePath)
    {
        var resourceUri = new ResourceUri(filePath);

        if (TextEditorConfig.RegisterModelFunc is null)
            return;

        await TextEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
            resourceUri,
            ServiceProvider));

        if (TextEditorConfig.TryRegisterViewModelFunc is not null)
        {
            var viewModelKey = await TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
                Key<TextEditorViewModel>.NewKey(),
                resourceUri,
                new TextEditorCategory("main"),
                false,
                ServiceProvider));

            if (viewModelKey != Key<TextEditorViewModel>.Empty &&
                TextEditorConfig.TryShowViewModelFunc is not null)
            {
                await TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
                    viewModelKey,
                    Key<TextEditorGroup>.Empty,
                    ServiceProvider));
            }
        }
    }
}