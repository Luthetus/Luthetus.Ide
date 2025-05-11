using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// <summary>
/// This type is thread safe.<br/><br/>
/// </summary>
public sealed class BackgroundTaskQueue
{
    /// <summary>
    /// The first item in this list, is the first item in the 'queue'.<br/><br/>
    /// The last item in this list, is the last item in the 'queue'.<br/><br/>
    /// ------------------------------------------------------------<br/><br/>
    /// The first item in this list, is the OLDEST item in the 'queue'<br/><br/>
    /// The last item in this list, is the MOST-RECENT item in the 'queue'<br/><br/>
    /// </summary>
    private readonly Queue<IBackgroundTaskGroup> _queue = new();
    /// <summary>
    /// Used when enqueueing.
    /// </summary>
    private readonly object _modifyQueueLock = new();

	public BackgroundTaskQueue(Key<BackgroundTaskQueue> key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }
    
	public Key<BackgroundTaskQueue> Key { get; }
    public string DisplayName { get; }

    /// <summary>
    /// Returns the amount of <see cref="IBackgroundTask"/>(s) in the queue.
    /// </summary>
    public int Count => _queue.Count;

    public SemaphoreSlim __DequeueSemaphoreSlim { get; } = new(0);

	public List<IBackgroundTaskGroup> GetBackgroundTaskList() => _queue.ToList();

    /// <summary>
    /// When enqueueing an event, a batchFunc is also provided.<br/><br/>
    /// 
    /// The batchFunc is to take the 'to-be-queued' entry, and decide if it can merge with the last event in the queue,
    /// as to make a batched event.<br/><br/>
    /// 
    /// This batchFunc is invoked over and over, until either a null 'batch event' is returned. Or, there are no more entries in
    /// the queue to merge with.<br/><br/>
    /// 
    /// When a null 'batch event' is returned, then the last item in the queue remains unchanged, and after it the 'to-be-queued'
    /// is ultimately queued.<br/><br/>
    /// 
    /// Each invocation of the 'batchFunc' will replace the 'to-be-queued' unless the 'batch event' returned was null.<br/><br/>
    /// </summary>
    public void Enqueue(IBackgroundTaskGroup downstreamEvent)
    {
		lock (_modifyQueueLock)
		{
			_queue.Enqueue(downstreamEvent);
			__DequeueSemaphoreSlim.Release();
		}
    }
    
    /// <summary>
    /// Returns the first entry in the queue, according to 'first in first out'
    /// </summary>
    public IBackgroundTaskGroup? __DequeueOrDefault()
    {
		lock (_modifyQueueLock)
		{
			if (_queue.Count <= 0)
				return null;

			return _queue.Dequeue();
		}
    }
}
