using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Concurrent;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

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
