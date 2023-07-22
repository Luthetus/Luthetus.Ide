using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Adhoc;

public partial class AdhocDisplay : ComponentBase, IDisposable
{
    [Inject]
    private ICommonBackgroundTaskQueue CommonBackgroundTaskQueue { get; set; } = null!;
    [Inject]
    private ICommonBackgroundTaskMonitor CommonBackgroundTaskMonitor { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private CancellationTokenSource _cancellationTokenSource = new();

    protected override void OnInitialized()
    {
        CommonBackgroundTaskMonitor.ExecutingBackgroundTaskChanged += BackgroundTaskMonitorOnExecutingBackgroundTaskChanged;

        base.OnInitialized();
    }

    private async void BackgroundTaskMonitorOnExecutingBackgroundTaskChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void EnqueueTaskOnClick()
    {
        _cancellationTokenSource.Cancel();
        var token = _cancellationTokenSource.Token;
        _cancellationTokenSource = new();

        var backgroundTask = new BackgroundTask(
            async cancellationToken => await WorkItem(cancellationToken),
            "Execute App",
            "Executes dotnet run",
            true,
            cancellationToken => Task.CompletedTask,
            Dispatcher,
            BackgroundTaskKey.NewBackgroundTaskKey(),
            token);

        CommonBackgroundTaskQueue.QueueBackgroundWorkItem(
            backgroundTask);
    }

    private async Task WorkItem(
        CancellationToken cancellationToken)
    {
        throw new ApplicationException("yayaya");
        cancellationToken.ThrowIfCancellationRequested();

        await Task.Delay(1_500, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
    }

    public void Dispose()
    {
        CommonBackgroundTaskMonitor.ExecutingBackgroundTaskChanged -= BackgroundTaskMonitorOnExecutingBackgroundTaskChanged;

        _cancellationTokenSource.Cancel();
    }
}