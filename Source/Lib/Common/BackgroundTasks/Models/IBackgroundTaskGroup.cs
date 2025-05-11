using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTaskGroup
{
	public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; }
    public Key<BackgroundTaskQueue> QueueKey { get; }
    public bool __TaskCompletionSourceWasCreated { get; set; }
    
    /// <summary>
    /// This method is the actual work item that gets awaited in order to handle the event.
    /// </summary>
	public ValueTask HandleEvent(CancellationToken cancellationToken);
}
