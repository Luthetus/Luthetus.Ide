using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskService : IBackgroundTaskService
{
    private readonly Dictionary<Key<BackgroundTaskQueue>, BackgroundTaskQueue> _queueContainerMap = new();

    private bool _enqueuesAreDisabled;

    public ImmutableArray<BackgroundTaskQueue> Queues => _queueContainerMap.Values.ToImmutableArray();

    public Task EnqueueAsync(IBackgroundTask backgroundTask)
    {
        // TODO: Could there be concurrency issues regarding '_enqueuesAreDisabled'? (2023-11-19)
        if (_enqueuesAreDisabled)
            return Task.CompletedTask;

        var queue = _queueContainerMap[backgroundTask.QueueKey];

        return queue.Queue.EnqueueAsync(backgroundTask);
    }

    public Task EnqueueAsync(Key<BackgroundTask> taskKey, Key<BackgroundTaskQueue> queueKey, string name, Func<Task> runFunc)
    {
        return EnqueueAsync(new BackgroundTask(taskKey, queueKey, name, runFunc));
    }

    public Task<IBackgroundTask> DequeueAsync(
        Key<BackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken)
    {
        var queue = _queueContainerMap[queueKey];
        return queue.Queue.DequeueOrDefaultAsync();
    }

    public void RegisterQueue(BackgroundTaskQueue queue)
    {
        _queueContainerMap.Add(queue.Key, queue);
    }

    public void SetExecutingBackgroundTask(
        Key<BackgroundTaskQueue> queueKey,
        IBackgroundTask? backgroundTask)
    {
        var queue = _queueContainerMap[queueKey];

        queue.ExecutingBackgroundTask = backgroundTask;
    }
    
    public BackgroundTaskQueue GetQueue(Key<BackgroundTaskQueue> queueKey)
    {
        return _queueContainerMap[queueKey];
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _enqueuesAreDisabled = true;

        // TODO: Polling solution for now, perhaps change to a more optimal solution? (2023-11-19)
        while (_queueContainerMap.Values.SelectMany(x => x.BackgroundTasks).Any())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        }
    }
}
