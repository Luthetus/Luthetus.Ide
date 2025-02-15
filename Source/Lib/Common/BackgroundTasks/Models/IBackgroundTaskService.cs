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
    
    /// <summary>
	/// Background tasks are currently very un-optimized because the 'IBackgroundTask' interface
	/// carries too much information and is causing boxing for structs.
	///
	/// One could use generics to avoid the struct boxing but it won't help much
	/// since the Queue would still need to box it as an IBackgroundTask.
	///
	/// Thus, 'IBackgroundTask' is being removed entirely, and being replaced with 'IBackgroundTaskGroup'.
	///
	/// This new interface will be very "open ended" with little definitions on the interface itself.
	///
	/// The 'IBackgroundTaskGroup' is expected to always be a reference type.
	///
	/// Then, the 'IBackgroundTaskGroup' goes on to
	/// handle its own inner queue(s).
	///
	/// The IBackgroundTaskService is just giving turns to each 'IBackgroundTaskGroup'.
	///
	/// The group itself will still need to be enqueue'd.
	/// 
	/// But the previously called 'IBackgroundTask' interface would have its queue
	/// as a property on the 'IBackgroundTaskGroup'.
	///
	/// By doing this the previous 'IBackgroundTask' queue can be a queue of whatever
	/// value type represents the event that occurred.
	///
	/// With respect to batching...
	/// If this is added, it would be added such that while the dequeue is of 'some group'
	/// then continue dequeueing and count the consecutive dequeues that were of 'some group'.
	/// |
	/// Then tell the group that it was dequeued consecutively 'count' times.
	/// Internally the group can decide what to do with this information.
	/// Perhaps it chooses to dequeue from its inner queue 'count' times.
	/// |
	/// Issue with the batching is that if it is done incorrectly then
	/// it is a massive performance loss.
	/// |
	/// So preferably if going to add it, do so at the very end once
	/// everything else is correct, and it is shown to be useful.
	///
	/// I can track the old code by references to the 'Enqueue' method group.
	///
	/// The new code will invoke 'EnqueueGroup'.
	///
	/// Once all the old code is moved over then the 'Enqueue' method group will be removed.
	/// </summary>
	public void EnqueueGroup(IBackgroundTaskGroup backgroundTaskGroup);
    
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
