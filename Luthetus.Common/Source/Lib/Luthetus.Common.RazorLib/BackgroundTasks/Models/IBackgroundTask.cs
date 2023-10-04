using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTask
{
    public Key<BackgroundTask> BackgroundTaskKey { get; }
    public Key<BackgroundTaskQueue> QueueKey { get; }
    public string Name { get; }
    public Task? WorkProgress { get; }

    internal Task InvokeWorkItem(CancellationToken cancellationToken);
}