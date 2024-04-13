using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

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
