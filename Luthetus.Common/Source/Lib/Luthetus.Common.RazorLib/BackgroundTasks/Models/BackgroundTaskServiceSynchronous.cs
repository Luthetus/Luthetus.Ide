using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskServiceSynchronous : IBackgroundTaskService
{
    /// <summary><see cref="BackgroundTaskServiceSynchronous"/> is used for unit testing. 
    /// As such, un-needed members are throwing a <see cref="NotImplementedException"/>.</summary>
    public IBackgroundTask? ExecutingBackgroundTask => throw new NotImplementedException();

    /// <summary><see cref="BackgroundTaskServiceSynchronous"/> is used for unit testing. 
    /// As such, un-needed members are throwing a <see cref="NotImplementedException"/>.</summary>
    public ImmutableArray<IBackgroundTask> PendingBackgroundTasks => throw new NotImplementedException();

    /// <summary><see cref="BackgroundTaskServiceSynchronous"/> is used for unit testing. 
    /// As such, un-needed members are throwing a <see cref="NotImplementedException"/>.</summary>
    public ImmutableArray<IBackgroundTask> CompletedBackgroundTasks => throw new NotImplementedException();

    public event Action? ExecutingBackgroundTaskChanged;

    public void Enqueue(IBackgroundTask backgroundTask)
    {
        backgroundTask
            .InvokeWorkItem(CancellationToken.None)
            .Wait();
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

    /// <summary><see cref="BackgroundTaskServiceSynchronous"/> is used for unit testing. 
    /// As such, un-needed members are throwing a <see cref="NotImplementedException"/>.</summary>
    public void SetExecutingBackgroundTask(
        Key<BackgroundTaskQueue> queueKey,
        IBackgroundTask? backgroundTask)
    {
        ExecutingBackgroundTaskChanged?.Invoke();
        throw new NotImplementedException();
    }

    public void RegisterQueue(BackgroundTaskQueue queue)
    {
        // This method should do nothing for this
        // implementation of IBackgroundTaskService
        return;
    }

    public int GetQueueCount(Key<BackgroundTaskQueue> queueKey)
    {
        // This method should do nothing for this
        // implementation of IBackgroundTaskService
        throw new NotImplementedException();
    }
}