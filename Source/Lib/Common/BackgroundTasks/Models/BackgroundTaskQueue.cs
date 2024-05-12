using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Reactives.Models;

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
    public ThrottleEventQueueAsync Queue { get; } = new();

    public IBackgroundTask? ExecutingBackgroundTask
    {
        get => _executingBackgroundTask;
        set
        {
            _executingBackgroundTask = value;
            ExecutingBackgroundTaskChanged?.Invoke();
        }
    }

    public int CountOfBackgroundTasks => Queue.Count;

	public ImmutableArray<IBackgroundTask> BackgroundTasks => Queue.ThrottleEventList;

    public event Action? ExecutingBackgroundTaskChanged;
}
