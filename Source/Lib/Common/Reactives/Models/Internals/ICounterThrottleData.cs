namespace Luthetus.Common.RazorLib.Reactives.Models.Internals;

public interface ICounterThrottleData
{
    public object IdLock { get; }
    public object ExecutedCountLock { get; }
    public Stack<Func<Task>> WorkItemStack { get; }
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

    /// <summary>
    /// The dialog can set this property in order to have a roundabout-hack for rendering
    /// a progress bar.
    /// </summary>
    public Func<double, Task>? HACK_ReRenderProgress { get; set; }
}
