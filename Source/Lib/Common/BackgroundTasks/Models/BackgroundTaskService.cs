using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskService : IBackgroundTaskService
{
    private readonly Dictionary<Key<IBackgroundTaskQueue>, BackgroundTaskQueue> _queueContainerMap = new();

    private bool _enqueuesAreDisabled;

    public ImmutableArray<IBackgroundTaskQueue> Queues => _queueContainerMap.Values.Select(x => (IBackgroundTaskQueue)x).ToImmutableArray();

    public void Enqueue(IBackgroundTask backgroundTask)
    {
        // TODO: Could there be concurrency issues regarding '_enqueuesAreDisabled'? (2023-11-19)
        if (_enqueuesAreDisabled)
            return;

        _queueContainerMap[backgroundTask.QueueKey]
			.Enqueue(backgroundTask);
    }

    public void Enqueue(Key<IBackgroundTask> taskKey, Key<IBackgroundTaskQueue> queueKey, string name, Func<Task> runFunc)
    {
        Enqueue(new BackgroundTask(taskKey, queueKey, name, runFunc));
    }

	public IBackgroundTask? Dequeue(Key<IBackgroundTaskQueue> queueKey)
    {
        var queue = _queueContainerMap[queueKey];
        return queue.DequeueOrDefault();
    }

    public async Task<IBackgroundTask?> DequeueAsync(
        Key<IBackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken)
    {
        var queue = _queueContainerMap[queueKey];
		await queue.DequeueSemaphoreSlim.WaitAsync();
        return queue.DequeueOrDefault();
    }

    public void RegisterQueue(IBackgroundTaskQueue queue)
    {
        _queueContainerMap.Add(queue.Key, (BackgroundTaskQueue)queue);
    }

    public void SetExecutingBackgroundTask(
        Key<IBackgroundTaskQueue> queueKey,
        IBackgroundTask? backgroundTask)
    {
        var queue = _queueContainerMap[queueKey];

        queue.ExecutingBackgroundTask = backgroundTask;
    }
    
    public IBackgroundTaskQueue GetQueue(Key<IBackgroundTaskQueue> queueKey)
    {
        return _queueContainerMap[queueKey];
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _enqueuesAreDisabled = true;

        // TODO: Polling solution for now, perhaps change to a more optimal solution? (2023-11-19)
        while (_queueContainerMap.Values.SelectMany(x => x.BackgroundTaskList).Any())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
        }
    }
}
