using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using System.Collections.Concurrent;

namespace Luthetus.Ide.ClassLib.CompilerServices.HostedServiceCase;

public class UnitTestsCompilerServiceBackgroundTaskQueue : ICompilerServiceBackgroundTaskQueue
{
    private readonly ConcurrentQueue<IBackgroundTask> _backgroundTasks = new();
    private readonly SemaphoreSlim _workItemsQueueSemaphoreSlim = new(0);

    public bool EnqueueIsDisabledForStopping { get; private set; }
    public int QueueCount => _backgroundTasks.Count;

    public void QueueBackgroundWorkItem(
        IBackgroundTask backgroundTask)
    {
        if (EnqueueIsDisabledForStopping)
            return;

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

    public void StopEnqueue()
    {
        EnqueueIsDisabledForStopping = true;
    }
}