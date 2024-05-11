namespace Luthetus.Common.RazorLib.Reactives.Models.Internals;

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
public abstract class CounterThrottleAsyncBase : ICounterThrottleAsync
{
    public CounterThrottleAsyncBase(TimeSpan throttleTimeSpan)
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

public class CTA_NoConfigureAwait : CounterThrottleAsyncBase
{
    public CTA_NoConfigureAwait(TimeSpan throttleTimeSpan)
        : base(throttleTimeSpan)
    {
    }

    public override async Task PushEvent(Func<Task> workItem)
    {
        int id;
        lock (IdLock)
        {
            // TODO: I want the _id to be unique, but I also wonder...
            //       ...if adding this 'lock' logic has any effect
            //       on all the async/thread things I'm looking into.
            id = ++GetId;
        }

        try
        {
            await WorkItemSemaphore.WaitAsync();

            WorkItemStack.Push(workItem);
            if (WorkItemStack.Count > 1)
                return;
        }
        finally
        {
            WorkItemSemaphore.Release();
        }

        var localDelayTask = DelayTask;

        _ = Task.Run(async () =>
        {
            PushEventStart_SynchronizationContext = SynchronizationContext.Current;
            PushEventStart_Thread = Thread.CurrentThread;
            PushEventStart_DateTimeTuple = (id, DateTime.UtcNow);

            await localDelayTask;
            DelayTask = Task.Delay(ThrottleTimeSpan);

            lock (ExecutedCountLock)
            {
                WorkItemsExecutedCount++;
            }

            Func<Task> popWorkItem;
            try
            {
                await WorkItemSemaphore.WaitAsync();

                if (WorkItemStack.Count == 0)
                    return;

                popWorkItem = WorkItemStack.Pop();
                WorkItemStack.Clear();
            }
            finally
            {
                WorkItemSemaphore.Release();
            }

            await popWorkItem.Invoke();

            PushEventEnd_Thread = Thread.CurrentThread;
            PushEventEnd_SynchronizationContext = SynchronizationContext.Current;
            PushEventEnd_DateTimeTuple = (id, DateTime.UtcNow);
        });
    }
}

public class CTA_WithConfigureAwait : CounterThrottleAsyncBase
{
    public CTA_WithConfigureAwait(TimeSpan throttleTimeSpan)
        : base(throttleTimeSpan)
    {
    }

    public override async Task PushEvent(Func<Task> workItem)
    {
        int id;
        lock (IdLock)
        {
            // TODO: I want the _id to be unique, but I also wonder...
            //       ...if adding this 'lock' logic has any effect
            //       on all the async/thread things I'm looking into.
            id = ++GetId;
        }

        try
        {
            await WorkItemSemaphore.WaitAsync().ConfigureAwait(false);

            WorkItemStack.Push(workItem);
            if (WorkItemStack.Count > 1)
                return;
        }
        finally
        {
            WorkItemSemaphore.Release();
        }

        var localDelayTask = DelayTask;

        _ = Task.Run(async () =>
        {
            PushEventStart_SynchronizationContext = SynchronizationContext.Current;
            PushEventStart_Thread = Thread.CurrentThread;
            PushEventStart_DateTimeTuple = (id, DateTime.UtcNow);

            await localDelayTask.ConfigureAwait(false);
            DelayTask = Task.Delay(ThrottleTimeSpan);

            lock (ExecutedCountLock)
            {
                WorkItemsExecutedCount++;
            }

            Func<Task> popWorkItem;
            try
            {
                await WorkItemSemaphore.WaitAsync().ConfigureAwait(false);

                if (WorkItemStack.Count == 0)
                    return;

                popWorkItem = WorkItemStack.Pop();
                WorkItemStack.Clear();
            }
            finally
            {
                WorkItemSemaphore.Release();
            }

            await popWorkItem.Invoke().ConfigureAwait(false);

            PushEventEnd_Thread = Thread.CurrentThread;
            PushEventEnd_SynchronizationContext = SynchronizationContext.Current;
            PushEventEnd_DateTimeTuple = (id, DateTime.UtcNow);
        }).ConfigureAwait(false);
    }
}
