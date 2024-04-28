using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.StateHasChangedBoundaries.Displays;

/// <summary>
/// This component groups child content so it can be re-rendered,
/// while NOT re-rendered the remaining sectioned off child content.
/// </summary>
public partial class StateHasChangedBoundary : ComponentBase
{
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    public async Task InvokeStateHasChangedAsync()
    {
        await InvokeAsync(StateHasChanged);
    }
}