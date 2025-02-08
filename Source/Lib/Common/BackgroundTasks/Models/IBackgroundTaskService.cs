using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTaskService
{
	/// <summary>
	/// Generally speaking: Presume that the ContinuousTaskWorker is "always ready" to run the next task that gets enqueued.
	///
	/// Similarly: If you think the task you are enqueueing will result in the previous statement being untrue, then you should use the IndefiniteTaskWorker.
	/// </summary>
	public BackgroundTaskWorker ContinuousTaskWorker { get; }
	/// <summary>
	/// Generally speaking: Presume that the IndefiniteTaskWorker is NOT ready to run the next task that gets enqueued.
	///
	/// Similarly: If you think the task you are enqueueing will finish "immediately" then you should use the ContinuousTaskWorker.
	/// </summary>
    public BackgroundTaskWorker IndefiniteTaskWorker { get; }
    
    public List<IBackgroundTaskQueue> GetQueues();

    public void Enqueue(IBackgroundTask backgroundTask);
    public void Enqueue(Key<IBackgroundTask> taskKey, Key<IBackgroundTaskQueue> queueKey, string name, Func<ValueTask> runFunc);
    
    public Task EnqueueAsync(IBackgroundTask backgroundTask);
    public Task EnqueueAsync(Key<IBackgroundTask> taskKey, Key<IBackgroundTaskQueue> queueKey, string name, Func<ValueTask> runFunc);

	public void CompleteTaskCompletionSource(Key<IBackgroundTask> taskKey);

    public void RegisterQueue(IBackgroundTaskQueue queue);

	public IBackgroundTask? Dequeue(Key<IBackgroundTaskQueue> queueKey);

    public Task<IBackgroundTask?> DequeueAsync(
        Key<IBackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken);

    public IBackgroundTaskQueue GetQueue(Key<IBackgroundTaskQueue> queueKey);
    
    public void SetContinuousTaskWorker(BackgroundTaskWorker continuousTaskWorker);
    public void SetIndefiniteTaskWorker(BackgroundTaskWorker indefiniteTaskWorker);
}
