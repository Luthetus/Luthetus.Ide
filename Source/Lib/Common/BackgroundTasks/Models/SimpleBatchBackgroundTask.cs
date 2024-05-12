using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// <summary>
/// This class uses mutable state, and therefore a new instance must be made
/// as opposed to re-using the same one.<br/><br/>
/// 
/// This class will take contiguous events of this same type, and combine them to
/// all to run in a foreach loop, provided they share
/// the same <see cref="Identifier"/>.
/// Foreach batching is not necessarily more effective than merging the tasks into 1 task.
/// But, for a general case, this type can do the bare minimum and just loop over them.
/// The intention here is, perhaps after a task one is doing some logic that slows the app,
/// being able to execute the tasks in a foreach loop would then avoid that slow logic running
/// foreach task, but instead until after the foreach is finished.
/// </summary>
public class SimpleBatchBackgroundTask : IBackgroundTask
{
    protected List<Func<CancellationToken, Task>> _workItemList;

    public SimpleBatchBackgroundTask(
            string name,
            string identifier,
            Func<CancellationToken, Task> workItem,
            TimeSpan? throttleTimeSpan = null)
        : this(name, identifier, new List<Func<CancellationToken, Task>>() { workItem }, throttleTimeSpan)
    {
    }

    public SimpleBatchBackgroundTask(
        string name,
        string identifier,
        List<Func<CancellationToken, Task>> workItemList,
        TimeSpan? throttleTimeSpan = null)
    {
        Name = name;
        Identifier = identifier;
        _workItemList = workItemList;
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
        if (oldEvent is not SimpleBatchBackgroundTask oldSimpleBatchBackgroundTask)
            return null;

        oldSimpleBatchBackgroundTask._workItemList.AddRange(_workItemList);
        oldSimpleBatchBackgroundTask.Name += '_' + Name;
        return oldSimpleBatchBackgroundTask;
    }

    public virtual async Task HandleEvent(CancellationToken cancellationToken)
    {
        foreach (var workItem in _workItemList)
        {
            await workItem.Invoke(cancellationToken).ConfigureAwait(false);
        }
    }
}
