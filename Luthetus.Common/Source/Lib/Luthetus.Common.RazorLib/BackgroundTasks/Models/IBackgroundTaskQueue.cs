using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTaskQueue
{
    public Key<BackgroundTaskQueue> Key { get; }
    public string DisplayName { get; }
    public int CountOfBackgroundTasks { get; }
	public ImmutableArray<IBackgroundTask> BackgroundTasks { get; }
    public IBackgroundTask? ExecutingBackgroundTask { get; }

    public event Action? ExecutingBackgroundTaskChanged;

	public void Enqueue(IBackgroundTask backgroundTask);
	public IBackgroundTask DequeueOrDefault();
}
