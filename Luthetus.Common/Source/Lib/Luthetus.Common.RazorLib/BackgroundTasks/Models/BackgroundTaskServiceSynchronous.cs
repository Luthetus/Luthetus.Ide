using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskServiceSynchronous : IBackgroundTaskService
{
    private readonly Dictionary<Key<BackgroundTaskQueue>, BackgroundTaskQueue> _queueMap = new();

    /// <summary><see cref="BackgroundTaskServiceSynchronous"/> is used for unit testing. 
    /// As such, un-needed members are throwing a <see cref="NotImplementedException"/>.</summary>
    public IBackgroundTask? ExecutingBackgroundTask => throw new NotImplementedException();

    public event Action? ExecutingBackgroundTaskChanged;

    public void Enqueue(IBackgroundTask backgroundTask)
    {
        var queue = _queueMap[backgroundTask.QueueKey];

        queue.BackgroundTasks.Enqueue(backgroundTask);

        SetExecutingBackgroundTask(backgroundTask.QueueKey, backgroundTask);

        backgroundTask
            .InvokeWorkItem(CancellationToken.None)
            .Wait();

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

    /// <summary><see cref="BackgroundTaskServiceSynchronous"/> is used for unit testing. 
    /// As such, un-needed members are throwing a <see cref="NotImplementedException"/>.</summary>
    public void SetExecutingBackgroundTask(
        Key<BackgroundTaskQueue> queueKey,
        IBackgroundTask? backgroundTask)
    {
        var queue = _queueMap[queueKey];

        queue.ExecutingBackgroundTask = backgroundTask;
        ExecutingBackgroundTaskChanged?.Invoke();
    }
}