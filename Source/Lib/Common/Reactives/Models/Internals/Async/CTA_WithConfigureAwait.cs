namespace Luthetus.Common.RazorLib.Reactives.Models.Internals.Async;

public class CTA_WithConfigureAwait : CTA_Base
{
    public CTA_WithConfigureAwait(TimeSpan throttleTimeSpan)
        : base(throttleTimeSpan)
    {
    }

    public override async Task PushEvent(Func<Task> workItem, Func<double, Task>? progressFunc = null)
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

            if (progressFunc is null)
            {
                await localDelayTask.ConfigureAwait(false);
            }
            else
            {
                await ThrottleProgressHelper
                    .DelayWithProgress(ThrottleTimeSpan, progressFunc)
                    .ConfigureAwait(false);
            }

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
