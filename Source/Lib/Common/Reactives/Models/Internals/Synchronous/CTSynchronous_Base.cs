namespace Luthetus.Common.RazorLib.Reactives.Models.Internals.Synchronous;

public abstract class CTSynchronous_Base : ICounterThrottleSynchronous
{
    public CTSynchronous_Base(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    public object WorkItemLock { get; protected set; } = new();
    public Stack<Func<Task>> WorkItemStack { get; protected set; } = new();
    public object IdLock { get; protected set; } = new();
    public object ExecutedCountLock { get; protected set; } = new();
    public Task DelayTask { get; protected set; } = Task.CompletedTask;
    public Task WorkItemTask { get; protected set; } = Task.CompletedTask;
    public TimeSpan ThrottleTimeSpan { get; protected set; }
    public SynchronizationContext? PushEventStart_SynchronizationContext { get; protected set; }
    public SynchronizationContext? PushEventEnd_SynchronizationContext { get; protected set; }
    public Thread? PushEventStart_Thread { get; protected set; }
    public Thread? PushEventEnd_Thread { get; protected set; }
    public (int Id, DateTime DateTime) PushEventStart_DateTimeTuple { get; protected set; } = (-1, DateTime.MinValue);
    public (int Id, DateTime DateTime) PushEventEnd_DateTimeTuple { get; protected set; } = (-1, DateTime.MinValue);
    public int WorkItemsExecutedCount { get; protected set; }
    public int GetId { get; protected set; }
    public Func<double, Task>? HACK_ReRenderProgress { get; set; }

    public abstract void PushEvent(
        Func<Task> workItem,
        Func<double, Task>? progressFunc = null,
        CancellationToken delayCancellationToken = default);
}
