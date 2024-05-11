using Luthetus.Common.RazorLib.Reactives.Models.Internals;
using Luthetus.Common.RazorLib.Reactives.Models.Internals.Async;

namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ThrottleWip : CTA_Base
{
    /// <summary>
    /// This <see cref="TimeSpan"/> represents '1000ms / 60 = 16.6ms', whether this is equivalent to 60fps is unknown.
    /// </summary>
    public static readonly TimeSpan Sixty_Frames_Per_Second = TimeSpan.FromMilliseconds(16.6);

    /// <summary>
    /// This <see cref="TimeSpan"/> represents '1000ms / 30 = 33.3ms', whether this is equivalent to 30fps is unknown.
    /// </summary>
    public static readonly TimeSpan Thirty_Frames_Per_Second = TimeSpan.FromMilliseconds(33.3);

    public ThrottleWip(TimeSpan throttleTimeSpan)
        : base(throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    public override async Task PushEvent(
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
        var localWorkItemTask = WorkItemTask;

        WorkItemTask = Task.Run(async () =>
        {
            await localWorkItemTask.ConfigureAwait(false);

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

            DelayTask = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(ThrottleTimeSpan, delayCancellationToken).ConfigureAwait(false);
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
        });
    }
}
