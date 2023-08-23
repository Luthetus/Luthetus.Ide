using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using System.Collections.Concurrent;

namespace Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;

public class FileSystemBackgroundTaskQueue : IFileSystemBackgroundTaskQueue
{
    private readonly ConcurrentQueue<IBackgroundTask> _backgroundTasks = new();
    private readonly SemaphoreSlim _workItemsQueueSemaphoreSlim = new(0);

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
}