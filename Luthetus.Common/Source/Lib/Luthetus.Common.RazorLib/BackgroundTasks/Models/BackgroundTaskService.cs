using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskService : IBackgroundTaskService
{
    private readonly Dictionary<Key<BackgroundTaskQueue>, BackgroundTaskQueue> _queueMap = new();

    public void Enqueue(IBackgroundTask backgroundTask)
    {
        var queue = _queueMap[backgroundTask.QueueKey];

        queue.BackgroundTasks.Enqueue(backgroundTask);
        queue.WorkItemsQueueSemaphoreSlim.Release();
    }

    public void Enqueue(
        Key<BackgroundTask> taskKey,
        Key<BackgroundTaskQueue> queueKey,
        string name,
        Func<Task> runFunc)
    {
        Enqueue(new BackgroundTask(taskKey, queueKey, name, runFunc));
    }

    public async Task<IBackgroundTask?> DequeueAsync(
        Key<BackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken)
    {
        var queue = _queueMap[queueKey];

        await queue.WorkItemsQueueSemaphoreSlim.WaitAsync(cancellationToken);
        _ = queue.BackgroundTasks.TryDequeue(out var backgroundTask);

        return backgroundTask;
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
    
    public int GetQueueCount(Key<BackgroundTaskQueue> queueKey)
    {
        var queue = _queueMap[queueKey];
        return queue.BackgroundTasks.Count;
    }
}
