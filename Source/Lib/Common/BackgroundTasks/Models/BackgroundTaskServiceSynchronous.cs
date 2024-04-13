using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskServiceSynchronous : IBackgroundTaskService
{
    private readonly Dictionary<Key<BackgroundTaskQueue>, BackgroundTaskQueue> _queueMap = new();

    private bool _enqueuesAreDisabled;

    public ImmutableArray<BackgroundTaskQueue> Queues => _queueMap.Values.ToImmutableArray();

    public void Enqueue(IBackgroundTask backgroundTask)
    {
        // TODO: Could there be concurrency issues regarding '_enqueuesAreDisabled'? (2023-11-19)
        if (_enqueuesAreDisabled)
            return;

        var queue = _queueMap[backgroundTask.QueueKey];

		// TODO: Why enqueue when no dequeue happens? Also StopAsync seems nonsensical
		// for the same reason. This is the synchronous version.
        queue.Enqueue(backgroundTask);

        SetExecutingBackgroundTask(backgroundTask.QueueKey, backgroundTask);

        backgroundTask
            .HandleEvent(CancellationToken.None)
            .Wait();

		// Don't await Task.Delay(backgroundTask.ThrottleTimeSpan) for
		// this BackgroundTaskServiceSynchronous implementation.

        SetExecutingBackgroundTask(backgroundTask.QueueKey, null);
    }

    public void Enqueue(
        Key<BackgroundTask> taskKey,
        Key<BackgroundTaskQueue> queueKey,
        string name,
        Func<Task> runFunc)
    {
        Enqueue(new BackgroundTask(taskKey, queueKey, name, runFunc));
    }

    public Task<IBackgroundTask?> DequeueAsync(
        Key<BackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(default(IBackgroundTask?));
    }

    public void RegisterQueue(BackgroundTaskQueue queue)
    {
        _queueMap.Add(queue.Key, queue);
    }

    public void SetExecutingBackgroundTask(
        Key<BackgroundTaskQueue> queueKey,
        IBackgroundTask? backgroundTask)
    {
        var queue = _queueMap[queueKey];

        queue.ExecutingBackgroundTask = backgroundTask;
    }

    public BackgroundTaskQueue GetQueue(Key<BackgroundTaskQueue> queueKey)
    {
        return _queueMap[queueKey];
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _enqueuesAreDisabled = true;

        // TODO: Polling solution for now, perhaps change to a more optimal solution? (2023-11-19)
        while (_queueMap.Values.SelectMany(x => x.BackgroundTasks).Any())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
        }
    }
}