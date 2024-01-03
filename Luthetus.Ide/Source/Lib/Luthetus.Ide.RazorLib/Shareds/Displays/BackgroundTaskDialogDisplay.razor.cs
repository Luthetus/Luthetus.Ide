using Luthetus.Ide.RazorLib.Shareds.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

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