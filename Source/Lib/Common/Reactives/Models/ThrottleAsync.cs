namespace Luthetus.Common.RazorLib.Reactives.Models;

/// <summary>
/// I have been considering deleting this class for quite some time (2024-08-05).
/// But, I think I see the use of an async throttle, versus a synchronous throttle.
///
/// It is the same scenario as a background task service which has
/// a synchronous enqueue, and an asynchronous enqueue.
///
/// If one if on the UI thread, they will likely want to use the synchronous version
/// of these types. Because the synchronous versions execute the code
/// on a "different thread" and won't block the UI.
///
/// But, perhaps one has already used 'Task.Run(async () => ...)' in order
/// to get off the UI thread.
///
/// If they then go on to invoke the synchronous version of the throttle,
/// they originally started a task on a "different thread",
/// which then went on to later start a task on a "different thread".
/// 
/// If one knows they're not on the "UI thread" then the 'ThrottleAsync'
/// could be preferable, because someone needs to await the work item.
/// And, it might reduce overhead if the non UI thread waits the work item,
/// instead of yet again creating ANOTHER task that will await the work item.
///
/// Also, in the case of the background task service having an async enqueue,
/// at times one wants to block until their work item is finished executing.
/// Otherwise there becomes the need to do some hacky logic were a second enqueue
/// is performed after the first, in order to notify oneself that the first enqueue
/// was completed.
/// </summary>
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

        var previousTask = WorkItemTask;

        WorkItemTask = Task.Run(async () =>
        {
            // Await the previous work item task.
            await previousTask.ConfigureAwait(false);

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

			await Task.WhenAll(
					popWorkItem.Invoke(CancellationToken.None),
					Task.Delay(ThrottleTimeSpan, CancellationToken.None))
				.ConfigureAwait(false);
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

            await Task.Delay(pollingTimeSpan.Value).ConfigureAwait(false);
        }

        await WorkItemTask.ConfigureAwait(false);
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