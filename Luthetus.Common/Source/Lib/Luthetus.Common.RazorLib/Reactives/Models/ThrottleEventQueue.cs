using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Reactives.Models;

/// <summary>
/// This type is not thread safe by design.<br/><br/>
/// The class instance is instead accessed from within a 'lock() { }' by external code.
/// </summary>
public class ThrottleEventQueue
{
    /// <summary>
    /// The first item in this list, is the first item in the 'queue'.<br/><br/>
    /// The last item in this list, is the last item in the 'queue'.<br/><br/>
    /// </summary>
    private readonly List<IBackgroundTask> _throttleEventList = new();

    /// <summary>
    /// Returns the amount of <see cref="IBackgroundTask"/>(s) in the queue.
    /// </summary>
    public int Count => _throttleEventList.Count;

	public ImmutableArray<IBackgroundTask> ThrottleEventList => _throttleEventList.ToImmutableArray();

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
    public void Enqueue(IBackgroundTask recentEvent)
    {
        for (int i = _throttleEventList.Count - 1; i >= 0; i--)
        {
            IBackgroundTask? oldEvent = _throttleEventList[i];
            var batchEvent = recentEvent.BatchOrDefault(oldEvent);

            if (batchEvent is null)
                break;

            _throttleEventList.RemoveAt(i);
            recentEvent = batchEvent;
        }
        
        _throttleEventList.Add(recentEvent);
    }
    
    /// <summary>
    /// Returns the first entry in the queue, according to 'first in first out'
    /// </summary>
    public IBackgroundTask? DequeueOrDefault()
    {
		if (_throttleEventList.Count <= 0)
			return null;

        var firstEvent = _throttleEventList[0];
        _throttleEventList.RemoveAt(0);
        return firstEvent;
    }
}
