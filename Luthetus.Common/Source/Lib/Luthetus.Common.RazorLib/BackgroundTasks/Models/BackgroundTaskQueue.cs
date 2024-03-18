using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// Goal: Change BackgroundTask to act similarly to IThrottleEvent #Step 400 (2024-03-11)
/// -------------------------------------------------------------------------------------
/// Now that IThrottleEvent has been deleted, I want to change BackgroundTaskQueue
/// to be more like 'ThrottleEventQueue'
///
/// I want to make a venn diagram (in text form) for 'BackgroundTaskQueue' and
/// 'ThrottleEventQueue'.
///
/// 'BackgroundTaskQueue' uniquely has:
/// 	- 
///
/// 'BackgroundTaskQueue' and 'ThrottleEventQueue' both have:
/// 	- SomeQueueType _throttleEventList
///	 	- For 'BackgroundTaskQueue' this is: ConcurrentQueue<IBackgroundTask> BackgroundTasks
/// 		- For 'ThrottleEventQueue' this is: List<IBackgroundTask> _throttleEventList
///
/// 'ThrottleEventQueue' uniquely has:
/// 	- 
///
/// The resulting type looks like:
/// 'ThrottleBackgroundTaskQueue'
/// 	- Inner workings of ThrottleEventQueue
/// 	- Don't have 'BackgroundTasks' instead the type is a queue in and of itself.
///
/// Goal: Change BackgroundTask to act similarly to IThrottleEvent #Step 500 (2024-03-11)
/// -------------------------------------------------------------------------------------
/// I want to comment out the entirety of 'ThrottleController' and see what errors appear.
///
/// Most ideally, I find that the errors are reasonable to fix, so I just fix them then and there.
///
/// If all goes well, I can then just delete the 'ThrottleController' source code.
///
/// I just got an idea. The TextEditorService has a 'Post()' method. I was thinking,
/// "well that breaks everything... how am I supposed to batch events if the 'Post()'
/// itself is too an event.
///
/// I can just have 'Post()' define its own even (now called IBackgroundTask)
/// and the batch method batches the inner event, which likely came from the UI,
/// and the 'Post()' event just wraps this new batched event instead of the singular one.
///
/// The 'Post()' event invoke can just invoke the inner event.
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
	/// <summary>
	/// <see cref="ThrottleEventQueue"/> stores the front of the queue at index 0
	/// of a private list. When enqueueing, the entry is added as the last entry in
	/// that private list.<br/<br/>
	///
	/// Therefore, if one begins enqueueing, then meanwhile performs a dequeue,
	/// an index out of range exception can occur. Because the dequeue removed
	/// the item at index 0, and now you are 1 index out of bounds.<br/<br/>
	///
	/// This refers specifically to 'batching' where the final item is
	/// overwritten with the batch result.<br/<br/>
	/// </summary>
	private readonly object _lockQueue = new();

    /// <summary>
    /// TODO: Concern - one could access this property by casting
    /// an <see cref="IBackgroundTaskQueue"/> as a <see cref="BackgroundTaskQueue"/>
    /// </summary>
    private ThrottleEventQueue _backgroundTasks { get; } = new();

    private IBackgroundTask? _executingBackgroundTask;

    public BackgroundTaskQueue(Key<BackgroundTaskQueue> key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    public Key<BackgroundTaskQueue> Key { get; }
    public string DisplayName { get; }
    /// <summary>
    /// Used when dequeueing.
    /// </summary>
    public SemaphoreSlim WorkItemAvailableSemaphoreSlim { get; } = new(0);
    /// <summary>
    /// Used when enqueueing.
    /// </summary>
    public SemaphoreSlim QueueSemaphoreSlim { get; } = new(1, 1);

    public IBackgroundTask? ExecutingBackgroundTask
    {
        get => _executingBackgroundTask;
        set
        {
            _executingBackgroundTask = value;
            ExecutingBackgroundTaskChanged?.Invoke();
        }
    }

    public int CountOfBackgroundTasks => _backgroundTasks.Count;

	public ImmutableArray<IBackgroundTask> BackgroundTasks => _backgroundTasks.ThrottleEventList;

    public event Action? ExecutingBackgroundTaskChanged;

	public void Enqueue(IBackgroundTask backgroundTask)
	{
		lock (_lockQueue)
		{
			_backgroundTasks.Enqueue(backgroundTask);
		}
	}

	public IBackgroundTask DequeueOrDefault()
	{
		lock (_lockQueue)
		{
			return _backgroundTasks.DequeueOrDefault();
		}
	}
}
