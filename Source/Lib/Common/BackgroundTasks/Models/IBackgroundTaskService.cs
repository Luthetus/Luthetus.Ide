using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTaskService
{
    public ImmutableArray<IBackgroundTaskQueue> Queues { get; }

    public void Enqueue(IBackgroundTask backgroundTask);
    public void Enqueue(Key<IBackgroundTask> taskKey, Key<IBackgroundTaskQueue> queueKey, string name, Func<Task> runFunc);
    public void RegisterQueue(IBackgroundTaskQueue queue);

	public IBackgroundTask? Dequeue(Key<IBackgroundTaskQueue> queueKey);

    public Task<IBackgroundTask?> DequeueAsync(
        Key<IBackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken);

    public void SetExecutingBackgroundTask(
        Key<IBackgroundTaskQueue> queueKey,
        IBackgroundTask? backgroundTask);

    public IBackgroundTaskQueue GetQueue(Key<IBackgroundTaskQueue> queueKey);

    public Task StopAsync(CancellationToken cancellationToken);
}
