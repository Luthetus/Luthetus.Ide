using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// <summary>
/// This class allows for easy creation of a task, that comes with "redundancy" checking in the queue.
/// That is, if when enqueueing an instance of this type, the last item in the queue is already an instance of this type
/// with the same <see cref="Identifier"/>, then this instance will overwrite the last item in
/// the queue, because the logic has no value if ran many times one after another, therefore, just take the most recent event.
/// </summary>
public class TakeMostRecentBackgroundTask : IBackgroundTask
{
    protected Func<CancellationToken, Task> _workItem;

    public TakeMostRecentBackgroundTask(
        string name,
        string identifier,
        Func<CancellationToken, Task> workItem,
        TimeSpan? throttleTimeSpan = null)
    {
        Name = name;
        Identifier = identifier;
        _workItem = workItem;
        ThrottleTimeSpan = throttleTimeSpan ?? TimeSpan.FromMilliseconds(100);
    }

    public virtual string Name { get; protected set; }
    public virtual string Identifier { get; protected set; }
    public virtual Key<BackgroundTask> BackgroundTaskKey { get; protected set; } = Key<BackgroundTask>.NewKey();
    public virtual Key<BackgroundTaskQueue> QueueKey { get; protected set; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public virtual TimeSpan ThrottleTimeSpan { get; protected set; }
    public virtual Task? WorkProgress { get; protected set; }

    public virtual IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is not TakeMostRecentBackgroundTask oldTakeMostRecentBackgroundTask)
        {
            // Keep both events
            return null;
        }

        if (oldTakeMostRecentBackgroundTask.Name == Name)
        {
            // Keep this event (via replacement)
            return this;
        }

        // Keep both events
        return null;
    }

    public virtual Task HandleEvent(CancellationToken cancellationToken)
    {
        return _workItem.Invoke(cancellationToken);
    }
}
