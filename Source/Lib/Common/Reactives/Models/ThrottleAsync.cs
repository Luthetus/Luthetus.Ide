namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ThrottleAsync
{
    public ThrottleAsync(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    public TimeSpan ThrottleTimeSpan { get; }
    public SemaphoreSlim WorkItemSemaphore { get; protected set; } = new(1, 1);
    public Stack<Func<CancellationToken, Task>> WorkItemStack { get; protected set; } = new();
    public Task DelayTask { get; protected set; } = Task.CompletedTask;
    public Task WorkItemTask { get; protected set; } = Task.CompletedTask;
    public bool IsStoppingFurtherPushes { get; private set; }

    public async Task PushEvent(Func<CancellationToken, Task> workItem)
    {
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
            // Await the previous work item task.
            await localWorkItemTask.ConfigureAwait(false);

            await localDelayTask.ConfigureAwait(false);

            DelayTask = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(ThrottleTimeSpan, CancellationToken.None).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    // Eat the task cancelled exception.
                }
            });

            Func<CancellationToken, Task> popWorkItem;
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

            await popWorkItem.Invoke(CancellationToken.None).ConfigureAwait(false);
        });
    }

    public async Task StopFurtherPushes()
    {
        try
        {
            await WorkItemSemaphore.WaitAsync().ConfigureAwait(false);
            IsStoppingFurtherPushes = true;
        }
        finally
        {
            WorkItemSemaphore.Release();
        }
    }

    /// <summary>
    /// This method awaits the last task prior to returning.<br/><br/>
    /// 
    /// This method does NOT prevent pushes while flushing.
    /// To do so, invoke <see cref="StopFurtherPushes()"/>
    /// prior to invoking this method.<br/><br/>
    /// 
    /// The implementation of this method is a polling solution
    /// (as of this comment (2024-05-09)).
    /// </summary>
    public async Task UntilIsEmpty(
        TimeSpan? pollingTimeSpan = null,
        CancellationToken cancellationToken = default)
    {
        pollingTimeSpan ??= TimeSpan.FromMilliseconds(333);

        while (!cancellationToken.IsCancellationRequested)
        {
            if (WorkItemStack.Count == 0)
                break;

            await Task.Delay(pollingTimeSpan.Value);
        }

        await WorkItemTask;
    }

    /// <summary>
    /// This <see cref="TimeSpan"/> represents '1000ms / 60 = 16.6ms', whether this is equivalent to 60fps is unknown.
    /// </summary>
    public static readonly TimeSpan Sixty_Frames_Per_Second = TimeSpan.FromMilliseconds(16.6);

    /// <summary>
    /// This <see cref="TimeSpan"/> represents '1000ms / 30 = 33.3ms', whether this is equivalent to 30fps is unknown.
    /// </summary>
    public static readonly TimeSpan Thirty_Frames_Per_Second = TimeSpan.FromMilliseconds(33.3);
}