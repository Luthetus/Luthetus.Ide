using Luthetus.Ide.RazorLib.Shareds.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

/// <summary>
/// TODO: Does this need an IDisposable to set BackgroundTaskDialogModel.ReRenderFuncAsync back to null?
/// </summary>
public partial class BackgroundTaskDialogDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public BackgroundTaskDialogModel BackgroundTaskDialogModel { get; set; } = null!;

    protected override void OnParametersSet()
    {
        BackgroundTaskDialogModel.ReRenderFuncAsync = async () => await InvokeAsync(StateHasChanged);
        base.OnParametersSet();
    }
}