using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using System.Collections.Concurrent;

namespace Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;

public class LuthetusIdeTerminalBackgroundTaskService : ILuthetusIdeTerminalBackgroundTaskService
{
    private readonly ConcurrentQueue<IBackgroundTask> _backgroundTasks = new();
    private readonly SemaphoreSlim _workItemsQueueSemaphoreSlim = new(0);

    public event Action? ExecutingBackgroundTaskChanged;

    public IBackgroundTask? ExecutingBackgroundTask { get; private set; }

    public void QueueBackgroundWorkItem(
        IBackgroundTask backgroundTask)
    {
        _backgroundTasks.Enqueue(backgroundTask);

        _workItemsQueueSemaphoreSlim.Release();
    }

    public async Task<IBackgroundTask?> DequeueAsync(
        CancellationToken cancellationToken)
    {
        IBackgroundTask? backgroundTask;

        try
        {
            await _workItemsQueueSemaphoreSlim.WaitAsync(cancellationToken);

            _backgroundTasks.TryDequeue(out backgroundTask);
        }
        finally
        {
            _workItemsQueueSemaphoreSlim.Release();
        }

        return backgroundTask;
    }

    public void SetExecutingBackgroundTask(IBackgroundTask? backgroundTask)
    {
        ExecutingBackgroundTask = backgroundTask;
        ExecutingBackgroundTaskChanged?.Invoke();
    }
}