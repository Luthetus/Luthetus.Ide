using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTaskQueue
{
    public Key<BackgroundTaskQueue> Key { get; }
    public string DisplayName { get; }
    public int CountOfBackgroundTasks { get; }
    public IBackgroundTask? ExecutingBackgroundTask { get; }

    public event Action? ExecutingBackgroundTaskChanged;
}
