namespace Luthetus.Common.RazorLib.Reactives.Models.Internals.Async;

/// <summary>
/// I refuse to not understand how this works...<br/><br/>
/// 
/// Brute force isn't my preferred way of action, but at this point
/// it doesn't seem like a bad idea.<br/><br/>
/// 
/// There are a limited amount of ways one can invoke async/Task related API.<br/><br/>
/// 
/// Therefore, I can create separate classes all of which iterate over every possible invocation.<br/><br/>
/// 
/// Then, I'll add a button per implementation, that renders a dialog, of which uses the throttle
/// when I drag the dialog.<br/><br/>
/// 
/// Inside the dialog I can then render out on the UI all information about the threads used,
/// SynchronizationContext(s) etc... as I'm dragging the dialog.<br/><br/>
/// 
/// Preferably by the end of this, I can create the simplest possible UI elements that demonstrate
/// the various ways of invoking async/Task API. If for some reason I ever forget
/// I can then reference these instead of doing this again.<br/><br/>
/// </summary>
public abstract class CTA_Base : ICounterThrottleAsync
{
    public CTA_Base(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    public SemaphoreSlim WorkItemSemaphore { get; protected set; } = new(1, 1);
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

    public abstract Task PushEvent(Func<Task> workItem);
}
