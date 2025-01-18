using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTaskQueue
{
    public Key<IBackgroundTaskQueue> Key { get; }
    public string DisplayName { get; }
    public int Count { get; }
	public ImmutableArray<IBackgroundTask> BackgroundTaskList { get; }
    public IBackgroundTask? ExecutingBackgroundTask { get; }

    public event Action? ExecutingBackgroundTaskChanged;
}
