using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// <summary>
/// This class allows for easy creation of a task, that has NO "redundancy" checking in the queue.
/// That is, if when enqueueing an instance of this type, there is a last item in the queue, then this type under no circumstances
/// will batch, replace, or other, with that existing queue'd item.
/// </summary>
/// <remarks>
/// There is nothing stopping someone from creating an implementation of a task
/// that sees the last item in the queue to be of this type, and then batching or etc with it.
/// One should NOT do this, but it is possible, and should be remarked about.
/// </remarks>
public class AsIsBackgroundTask : IBackgroundTask
{
    protected Func<CancellationToken, Task> _workItem;

    public AsIsBackgroundTask(
        string name,
        Func<CancellationToken, Task> workItem,
        TimeSpan? throttleTimeSpan = null)
    {
        Name = name;
        _workItem = workItem;
        ThrottleTimeSpan = throttleTimeSpan ?? TimeSpan.FromMilliseconds(100);
    }

    public virtual string Name { get; protected set; }
    public virtual Key<BackgroundTask> BackgroundTaskKey { get; protected set; } = Key<BackgroundTask>.NewKey();
    public virtual Key<BackgroundTaskQueue> QueueKey { get; protected set; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public virtual TimeSpan ThrottleTimeSpan { get; protected set; }
    public virtual Task? WorkProgress { get; protected set; }

    public virtual IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        // Keep both events
        return null;
    }

    public virtual Task HandleEvent(CancellationToken cancellationToken)
    {
        return _workItem.Invoke(cancellationToken);
    }
}
