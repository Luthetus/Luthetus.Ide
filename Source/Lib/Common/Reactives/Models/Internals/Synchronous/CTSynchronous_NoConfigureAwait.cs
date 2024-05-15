﻿using Luthetus.Common.RazorLib.Reactives.Models.Internals.Synchronous;

namespace Luthetus.Common.RazorLib.Reactives.Models.Internals;

public class CTSynchronous_NoConfigureAwait : CTSynchronous_Base
{
    public CTSynchronous_NoConfigureAwait(TimeSpan throttleTimeSpan)
        : base(throttleTimeSpan)
    {
        
    }

    public override void PushEvent(
        Func<Task> workItem,
        Func<double, Task>? progressFunc = null,
        CancellationToken delayCancellationToken = default)
    {
        int id;
        lock (IdLock)
        {
            // TODO: I want the _id to be unique, but I also wonder...
            //       ...if adding this 'lock' logic has any effect
            //       on all the async/thread things I'm looking into.
            id = ++GetId;
        }

        PushEventStart_SynchronizationContext = SynchronizationContext.Current;
        PushEventStart_Thread = Thread.CurrentThread;
        PushEventStart_DateTimeTuple = (id, DateTime.UtcNow);

        lock (WorkItemLock)
        {
            WorkItemStack.Push(workItem);
            if (WorkItemStack.Count > 1)
                return;
        }

        var localDelayTask = DelayTask;

        _ = Task.Run(async () =>
        {
            if (progressFunc is null)
            {
                await localDelayTask;
            }
            else
            {
                await ThrottleProgressHelper
                    .DelayWithProgress(ThrottleTimeSpan, progressFunc);
            }

            DelayTask = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(ThrottleTimeSpan, delayCancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Eat the task cancelled exception.
                }
            });

            lock (ExecutedCountLock)
            {
                WorkItemsExecutedCount++;
            }

            Func<Task> popWorkItem;
            lock (WorkItemLock)
            {
                if (WorkItemStack.Count == 0)
                    return;

                popWorkItem = WorkItemStack.Pop();
                WorkItemStack.Clear();
            }

            await popWorkItem.Invoke();

            PushEventEnd_Thread = Thread.CurrentThread;
            PushEventEnd_SynchronizationContext = SynchronizationContext.Current;
            PushEventEnd_DateTimeTuple = (id, DateTime.UtcNow);
        });
    }
}