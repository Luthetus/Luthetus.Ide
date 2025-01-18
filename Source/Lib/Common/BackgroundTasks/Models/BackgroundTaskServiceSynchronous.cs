using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskServiceSynchronous : IBackgroundTaskService
{
    private readonly Dictionary<Key<IBackgroundTaskQueue>, BackgroundTaskQueue> _queueMap = new();

    private bool _enqueuesAreDisabled;

    public ImmutableArray<IBackgroundTaskQueue> Queues => _queueMap.Values.Select(x => (IBackgroundTaskQueue)x).ToImmutableArray();

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

		// TODO: This is probably a bad idea, but I don't know if BackgroundTaskServiceSynchronous is even used anymore...
		//       ...and it is currently stopping the build so I'm doing this for now. (2025-01-18)
        _ = Task.Run(async () => await backgroundTask.HandleEvent(CancellationToken.None));

		// Don't await Task.Delay(backgroundTask.ThrottleTimeSpan) for
		// this BackgroundTaskServiceSynchronous implementation.

        SetExecutingBackgroundTask(backgroundTask.QueueKey, null);
    }

    public void Enqueue(Key<IBackgroundTask> taskKey, Key<IBackgroundTaskQueue> queueKey, string name, Func<ValueTask> runFunc)
    {
        Enqueue(new BackgroundTask(taskKey, queueKey, name, runFunc));
    }
    
    public Task EnqueueAsync(IBackgroundTask backgroundTask)
    {
    	Enqueue(backgroundTask);
    	return Task.CompletedTask;
    }
    
    public Task EnqueueAsync(Key<IBackgroundTask> taskKey, Key<IBackgroundTaskQueue> queueKey, string name, Func<ValueTask> runFunc)
    {
    	return EnqueueAsync(new BackgroundTask(taskKey, queueKey, name, runFunc));
    }
    
	public void CompleteTaskCompletionSource(Key<IBackgroundTask> taskKey)
	{
		throw new NotImplementedException($"The {nameof(CompleteTaskCompletionSource)}(...) method should not be invoked when using " +
            $"a {nameof(BackgroundTaskServiceSynchronous)}. This type is designed such that {nameof(Enqueue)}(...) " +
            $"will invoke {nameof(Task.Wait)}() on the background task, as opposed to enqueueing.");
	}

	public IBackgroundTask? Dequeue(Key<IBackgroundTaskQueue> queueKey)
    {
        throw new NotImplementedException($"The {nameof(Dequeue)}(...) method should not be invoked when using " +
            $"a {nameof(BackgroundTaskServiceSynchronous)}. This type is designed such that {nameof(Enqueue)}(...) " +
            $"will invoke {nameof(Task.Wait)}() on the background task, as opposed to enqueueing.");
    }

    public Task<IBackgroundTask?> DequeueAsync(
        Key<IBackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"The {nameof(DequeueAsync)}(...) method should not be invoked when using " +
            $"a {nameof(BackgroundTaskServiceSynchronous)}. This type is designed such that {nameof(Enqueue)}(...) " +
            $"will invoke {nameof(Task.Wait)}() on the background task, as opposed to enqueueing.");
    }

    public void RegisterQueue(IBackgroundTaskQueue queue)
    {
        _queueMap.Add(queue.Key, (BackgroundTaskQueue)queue);
    }

    public void SetExecutingBackgroundTask(
        Key<IBackgroundTaskQueue> queueKey,
        IBackgroundTask? backgroundTask)
    {
        var queue = _queueMap[queueKey];

        queue.ExecutingBackgroundTask = backgroundTask;
    }

    public IBackgroundTaskQueue GetQueue(Key<IBackgroundTaskQueue> queueKey)
    {
        return _queueMap[queueKey];
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _enqueuesAreDisabled = true;

        // TODO: Polling solution for now, perhaps change to a more optimal solution? (2023-11-19)
        while (_queueMap.Values.SelectMany(x => x.BackgroundTaskList).Any())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
        }
    }
}