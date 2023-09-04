using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.BackgroundServiceCase;

public partial class BackgroundServiceDisplay : ComponentBase, IDisposable
{
    [Parameter, EditorRequired]
    public IBackgroundTaskService BackgroundTaskQueue { get; set; } = null!;
    [Parameter, EditorRequired]
    public string DisplayName { get; set; } = null!;

    protected override void OnInitialized()
    {
        BackgroundTaskQueue.ExecutingBackgroundTaskChanged += BackgroundTaskQueue_ExecutingBackgroundTaskChanged;
        base.OnInitialized();
    }

    private async void BackgroundTaskQueue_ExecutingBackgroundTaskChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        BackgroundTaskQueue.ExecutingBackgroundTaskChanged -= BackgroundTaskQueue_ExecutingBackgroundTaskChanged;
    }
}