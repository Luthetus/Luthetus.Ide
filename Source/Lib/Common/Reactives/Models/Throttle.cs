namespace Luthetus.Common.RazorLib.Reactives.Models;

public class Throttle
{
    private readonly object _lockWorkItems = new();
	private readonly Stack<Func<CancellationToken, Task>> _workItemStack = new();

    public Throttle(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

	public Task _workItemTask = Task.CompletedTask;

    public TimeSpan ThrottleTimeSpan { get; }
	
	public void Run(Func<CancellationToken, Task> workItem)
    {
		lock (_lockWorkItems)
		{
			_workItemStack.Push(workItem);
            if (_workItemStack.Count > 1)
                return;
		}

        var previousTask = _workItemTask;

        _workItemTask = Task.Run(async () =>
        {
            // Await the previous work item task.
            await previousTask.ConfigureAwait(false);

            Func<CancellationToken, Task> popWorkItem;
            lock (_lockWorkItems)
            {
                if (_workItemStack.Count == 0)
                    return;

                popWorkItem = _workItemStack.Pop();
                _workItemStack.Clear();
            }

			await Task.WhenAll(
					popWorkItem.Invoke(CancellationToken.None),
					Task.Delay(ThrottleTimeSpan))
				.ConfigureAwait(false);
        });
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