using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// <summary>
/// This type is thread safe.<br/><br/>
/// </summary>
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    /// <summary>
    /// The first item in this list, is the first item in the 'queue'.<br/><br/>
    /// The last item in this list, is the last item in the 'queue'.<br/><br/>
    /// ------------------------------------------------------------<br/><br/>
    /// The first item in this list, is the OLDEST item in the 'queue'<br/><br/>
    /// The last item in this list, is the MOST-RECENT item in the 'queue'<br/><br/>
    /// </summary>
    private readonly LinkedList<IBackgroundTask> _queue = new();
    /// <summary>
    /// Used when enqueueing.
    /// </summary>
    private readonly object _modifyQueueLock = new();

	public BackgroundTaskQueue(Key<IBackgroundTaskQueue> key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

	public Key<IBackgroundTaskQueue> Key { get; }
    public string DisplayName { get; }

    /// <summary>
    /// Returns the amount of <see cref="IBackgroundTask"/>(s) in the queue.
    /// </summary>
    public int Count => _queue.Count;

	public ImmutableArray<IBackgroundTask> BackgroundTaskList => _queue.ToImmutableArray();

    public SemaphoreSlim __DequeueSemaphoreSlim { get; } = new(0);

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
    public void Enqueue(IBackgroundTask downstreamEvent)
    {
		lock (_modifyQueueLock)
		{
			if (downstreamEvent.EarlyBatchEnabled && _queue.Count > 0)
			{
				var upstreamEvent = _queue.Last.Value;
				// TODO: Rename 'BatchOrDefault' to 'TryMergeIntoUpstream'
				var batchEvent = downstreamEvent.EarlyBatchOrDefault(upstreamEvent);

				if (batchEvent is not null)
                {
					// The length of the queue has not changed,
					// so do not release the dequeue semaphore here.
					//
					// The batching was successful so return early.
					_queue.RemoveLast();
					_queue.AddLast(batchEvent);
            		return;
				}
			}

			// The batching was NOT successful so add to the queue.
			_queue.AddLast(downstreamEvent);
			__DequeueSemaphoreSlim.Release();
		}
    }
    
    /// <summary>
    /// Returns the first entry in the queue, according to 'first in first out'
    /// </summary>
    public IBackgroundTask? __DequeueOrDefault()
    {
		lock (_modifyQueueLock)
		{
			if (_queue.Count <= 0)
				return null;

			var task = _queue.First.Value;
            _queue.RemoveFirst();
			return task;
		}
    }
}
