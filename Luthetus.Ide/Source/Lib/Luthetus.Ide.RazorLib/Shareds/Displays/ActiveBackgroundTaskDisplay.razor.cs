using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class ActiveBackgroundTaskDisplay : IDisposable
{
    [Inject]
    public IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    private BackgroundTaskQueue _continuousBackgroundTaskWorker = null!;

    protected override void OnInitialized()
    {
        _continuousBackgroundTaskWorker = BackgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
        _continuousBackgroundTaskWorker.ExecutingBackgroundTaskChanged += ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;

        base.OnInitialized();
    }

    private async void ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _continuousBackgroundTaskWorker.ExecutingBackgroundTaskChanged -= ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;
    }
}