using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Reactives.Models;

/// <summary>
/// This type is thread safe.<br/><br/>
/// </summary>
public class ThrottleEventQueueAsync
{
    /// <summary>
    /// The first item in this list, is the first item in the 'queue'.<br/><br/>
    /// The last item in this list, is the last item in the 'queue'.<br/><br/>
    /// ------------------------------------------------------------<br/><br/>
    /// The first item in this list, is the OLDEST item in the 'queue'<br/><br/>
    /// The last item in this list, is the MOST-RECENT item in the 'queue'<br/><br/>
    /// </summary>
    private readonly List<IBackgroundTask> _throttleEventList = new();
    /// <summary>
    /// Used when dequeueing.
    /// </summary>
    private readonly SemaphoreSlim _dequeueSemaphoreSlim = new(0);
    /// <summary>
    /// Used when enqueueing.
    /// </summary>
    private readonly SemaphoreSlim _modifyQueueSemaphoreSlim = new(1, 1);

    private int _enqueueCounter;
    private int _dequeueCounter;

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
    public async Task EnqueueAsync(IBackgroundTask recentEvent)
    {
        _enqueueCounter++;
        Console.Write("e");
        try
        {
            await _modifyQueueSemaphoreSlim.WaitAsync();

            var queueLengthIncreased = true;
        
            for (int i = _throttleEventList.Count - 1; i >= 0; i--)
            {
                IBackgroundTask? oldEvent = _throttleEventList[i];
                var batchEvent = recentEvent.BatchOrDefault(oldEvent);

                if (batchEvent is null)
                    break;

                // In this case, either the current event stays,
                // or it is replaced with the new event.
                //
                // Therefore the que length does not change.
                queueLengthIncreased = false;

                _throttleEventList.RemoveAt(i);
                recentEvent = batchEvent;
            }
        
            _throttleEventList.Add(recentEvent);

            if (queueLengthIncreased)
                _dequeueSemaphoreSlim.Release();
        }
        finally
        {
            _modifyQueueSemaphoreSlim.Release();
        }
    }
    
    /// <summary>
    /// Returns the first entry in the queue, according to 'first in first out'
    /// </summary>
    public async Task<IBackgroundTask> DequeueOrDefaultAsync()
    {
        _dequeueCounter++;
        Console.Write("d");
        await _dequeueSemaphoreSlim.WaitAsync();

        try
        {
            await _modifyQueueSemaphoreSlim.WaitAsync();
		    
            var firstEvent = _throttleEventList[0];
            _throttleEventList.RemoveAt(0);
            return firstEvent;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw;
        }
        finally
        {
            _modifyQueueSemaphoreSlim.Release();
        }
    }
}
