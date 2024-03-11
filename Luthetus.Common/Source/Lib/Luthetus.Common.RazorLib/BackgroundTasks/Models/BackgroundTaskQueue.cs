using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Concurrent;

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
/// I want to change 
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private IBackgroundTask? _executingBackgroundTask;

    public BackgroundTaskQueue(Key<BackgroundTaskQueue> key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    public Key<BackgroundTaskQueue> Key { get; }
    public string DisplayName { get; }

    /// <summary>
    /// TODO: Concern - one could access this property by casting
    /// an <see cref="IBackgroundTaskQueue"/> as a <see cref="BackgroundTaskQueue"/>
    /// </summary>
    public ConcurrentQueue<IBackgroundTask> BackgroundTasks { get; } = new();
    /// <summary>
    /// TODO: Concern - one could access this property by casting
    /// an <see cref="IBackgroundTaskQueue"/> as a <see cref="BackgroundTaskQueue"/>
    /// </summary>
    public SemaphoreSlim WorkItemsQueueSemaphoreSlim { get; } = new(0);

    public IBackgroundTask? ExecutingBackgroundTask
    {
        get => _executingBackgroundTask;
        set
        {
            _executingBackgroundTask = value;
            ExecutingBackgroundTaskChanged?.Invoke();
        }
    }

    public int CountOfBackgroundTasks => BackgroundTasks.Count;

    public event Action? ExecutingBackgroundTaskChanged;
}
