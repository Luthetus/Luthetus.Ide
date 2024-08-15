namespace Luthetus.Common.RazorLib.Reactives.Models;

/// <summary>
/// ThrottleAsync does not achieve the desired result.
/// It blocks itself, and therefore is useless?
/// I like the idea I had with a blocking background task.
/// But it doesn't really extend here much, at least given how its currently written?
///
/// Okay, I see, ThrottleAsync works SOLELY from the idea that you are
/// using a fire and forget Task.Run and want to throttle the logic within
/// the Task.Run at a top level. But you cannot use it inside a foreach loop
/// because you are awaiting yourself.
///
/// In short ThrottleAsync is actually useful... because it keeps two invocations
/// of code running concurrently. Because one has to wait for the other to finish.
///
/// And that many Task.Run will cancel out to the most recent one.
/// But it still seems quite asinine?
/// </summary>
public class ThrottleAsync
{
	private readonly object _lockWorkItems = new();
	
    public ThrottleAsync(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    public TimeSpan ThrottleTimeSpan { get; }
    public Stack<Func<CancellationToken, Task>> WorkItemStack { get; protected set; } = new();
    public Task WorkItemTask { get; protected set; } = Task.CompletedTask;
    public bool IsStoppingFurtherPushes { get; private set; }

    public Task RunAsync(Func<CancellationToken, Task> workItem)
    {
    	lock (_lockWorkItems)
		{
			WorkItemStack.Push(workItem);
            if (WorkItemStack.Count > 1)
                return Task.CompletedTask;
		}
    
        var previousTask = WorkItemTask;
        WorkItemTask = ExecuteAsync(previousTask);
        return WorkItemTask;
    }
    
    private async Task ExecuteAsync(Task previousTask)
    {
    	// Await the previous work item task.
        await previousTask.ConfigureAwait(false);

		Func<CancellationToken, Task> popWorkItem;
        lock (_lockWorkItems)
        {
            if (WorkItemStack.Count == 0)
                return;

            popWorkItem = WorkItemStack.Pop();
            WorkItemStack.Clear();
        }

		await Task.WhenAll(
				popWorkItem.Invoke(CancellationToken.None),
				Task.Delay(ThrottleTimeSpan, CancellationToken.None))
			.ConfigureAwait(false);
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
}