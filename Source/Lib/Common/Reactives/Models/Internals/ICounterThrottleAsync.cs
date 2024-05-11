namespace Luthetus.Common.RazorLib.Reactives.Models.Internals;

public interface ICounterThrottleAsync
{
    public SemaphoreSlim WorkItemSemaphore { get; }
    public Stack<Func<Task>> WorkItemStack { get; }
    public object IdLock { get; }
    public object ExecutedCountLock { get; }

    public Task DelayTask { get; }
    public Task WorkItemTask { get; }

    public TimeSpan ThrottleTimeSpan { get; }
    public SynchronizationContext? PushEventStart_SynchronizationContext { get; }
    public SynchronizationContext? PushEventEnd_SynchronizationContext { get; }
    public Thread? PushEventStart_Thread { get; }
    public Thread? PushEventEnd_Thread { get; }
    public (int Id, DateTime DateTime) PushEventStart_DateTimeTuple { get; }
    public (int Id, DateTime DateTime) PushEventEnd_DateTimeTuple { get; }

    public int WorkItemsExecutedCount { get; }

    public int GetId { get; }

    public Task PushEvent(Func<Task> workItem);
}
