using Luthetus.Common.RazorLib.BackgroundTaskCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Adhoc;

public partial class AdhocDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IBackgroundTaskQueue BackgroundTaskQueue { get; set; } = null!;
    [Inject]
    private IBackgroundTaskMonitor BackgroundTaskMonitor { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private CancellationTokenSource _cancellationTokenSource = new();

    protected override void OnInitialized()
    {
        BackgroundTaskMonitor.ExecutingBackgroundTaskChanged += BackgroundTaskMonitorOnExecutingBackgroundTaskChanged;

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

        BackgroundTaskQueue.QueueBackgroundWorkItem(
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
        BackgroundTaskMonitor.ExecutingBackgroundTaskChanged -= BackgroundTaskMonitorOnExecutingBackgroundTaskChanged;

        _cancellationTokenSource.Cancel();
    }
}