using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;

public class LuthetusIdeFileSystemBackgroundTaskService : ILuthetusIdeFileSystemBackgroundTaskService
{
    private readonly ConcurrentQueue<IBackgroundTask> _backgroundTasks = new();
    private readonly SemaphoreSlim _workItemsQueueSemaphoreSlim = new(0);
    private readonly object _backgroundTaskListsLock = new();
    private readonly List<IBackgroundTask> _pendingBackgroundTasks = new();
    private readonly List<IBackgroundTask> _completedBackgroundTasks = new();

    public event Action? ExecutingBackgroundTaskChanged;

    public IBackgroundTask? ExecutingBackgroundTask { get; private set; }

    public ImmutableArray<IBackgroundTask> PendingBackgroundTasks
    {
        get
        {
            lock (_backgroundTaskListsLock)
            {
                return _pendingBackgroundTasks.ToImmutableArray();
            }
        }
    }

    public ImmutableArray<IBackgroundTask> CompletedBackgroundTasks
    {
        get
        {
            lock (_backgroundTaskListsLock)
            {
                return _completedBackgroundTasks.ToImmutableArray();
            }
        }
    }

    public void QueueBackgroundWorkItem(IBackgroundTask backgroundTask)
    {
        _backgroundTasks.Enqueue(backgroundTask);

        lock (_backgroundTaskListsLock)
        {
            _pendingBackgroundTasks.Add(backgroundTask);
        }

        _workItemsQueueSemaphoreSlim.Release();
    }

    public async Task<IBackgroundTask?> DequeueAsync(CancellationToken cancellationToken)
    {
        await _workItemsQueueSemaphoreSlim.WaitAsync(cancellationToken);
        _ = _backgroundTasks.TryDequeue(out var backgroundTask);

        lock (_backgroundTaskListsLock)
        {
            var indexOfBackgroundTask = _pendingBackgroundTasks.FindIndex(
                x => x.BackgroundTaskKey == backgroundTask.BackgroundTaskKey);

            _pendingBackgroundTasks.RemoveAt(indexOfBackgroundTask);
            _completedBackgroundTasks.Add(backgroundTask);
        }

        return backgroundTask;
    }

    public void SetExecutingBackgroundTask(IBackgroundTask? backgroundTask)
    {
        ExecutingBackgroundTask = backgroundTask;
        ExecutingBackgroundTaskChanged?.Invoke();
    }
}
