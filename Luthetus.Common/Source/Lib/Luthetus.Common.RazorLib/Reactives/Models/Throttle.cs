namespace Luthetus.Common.RazorLib.Reactives.Models;

public class Throttle : IThrottle
{
    private readonly object _lockWorkItemsStack = new();
    private readonly Stack<Func<CancellationToken, Task>> _workItemsStack = new();
    /// <summary>
    /// This semaphore is used to prevent a race condition.
    /// It occurs when assigning to <see cref="_previousWorkItemTask"/>.
    /// This is because, the assignment is done outside of the <see cref="_lockWorkItemsStack"/>.<br/><br/>
    /// 
    /// So, upon freeing the <see cref="_lockWorkItemsStack"/> one could invoke <see cref="FireAndForget(Func{CancellationToken, Task})"/>
    /// and then go on to access the lock to push their work item on to the stack.<br/><br/>
    /// 
    /// After that, one can do the 'Task.Run' invocation.
    /// If that 'Task.Run' invocation then starts and await <see cref="_previousWorkItemTask"/>,
    /// a race condition occurred.<br/><br/>
    /// 
    /// This race condition results in two work items running concurrently when the model
    /// is supposed to ensure <see cref="ShouldWaitForPreviousWorkItemToComplete"/>.<br/><br/>
    /// 
    /// The reason for not having the <see cref="_previousWorkItemTask"/> assignment inside the
    /// <see cref="_lockWorkItemsStack"/> lock, is because it is believed that one could
    /// fire and NOT-await the <see cref="_previousWorkItemTask"/> task.<br/><br/>
    /// 
    /// Then, for whatever reason, the <see cref="_previousWorkItemTask"/> immediately
    /// might take over the thread. Perhaps this then goes on to
    /// invoke <see cref="FireAndForget(Func{CancellationToken, Task})"/> from a different
    /// threat somehow, and a deadlock on the <see cref="_lockWorkItemsStack"/> stack occurs.
    /// </summary>
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    private CancellationTokenSource _throttleCancellationTokenSource = new();
    private Task _throttleDelayTask = Task.CompletedTask;
    private Task _previousWorkItemTask = Task.CompletedTask;

    public bool ShouldWaitForPreviousWorkItemToComplete { get; } = true;

    public Throttle(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    /// <summary>
    /// The default value for <see cref="ShouldWaitForPreviousWorkItemToComplete"/> is true
    /// </summary>
    public Throttle(TimeSpan throttleTimeSpan, bool shouldWaitForPreviousWorkItemToComplete)
    {
        ThrottleTimeSpan = throttleTimeSpan;
        ShouldWaitForPreviousWorkItemToComplete = shouldWaitForPreviousWorkItemToComplete;
    }

    public TimeSpan ThrottleTimeSpan { get; }

    public void FireAndForget(Func<CancellationToken, Task> workItem)
    {
        lock (_lockWorkItemsStack)
        {
            _workItemsStack.Push(workItem);

            if (_workItemsStack.Count > 1)
                return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                await _throttleDelayTask.ConfigureAwait(false);

                if (ShouldWaitForPreviousWorkItemToComplete)
                    await _previousWorkItemTask.ConfigureAwait(false);

                CancellationToken cancellationToken;
                Func<CancellationToken, Task> mostRecentWorkItem;

                lock (_lockWorkItemsStack)
                {
                    mostRecentWorkItem = _workItemsStack.Pop();
                    _workItemsStack.Clear();

                    _throttleCancellationTokenSource.Cancel();
                    _throttleCancellationTokenSource = new();
                    cancellationToken = _throttleCancellationTokenSource.Token;
                }

                _throttleDelayTask = Task.Run(async () =>
                {
                    await Task.Delay(ThrottleTimeSpan).ConfigureAwait(false);
                }, CancellationToken.None);

                _previousWorkItemTask = Task.Run(async () =>
                {
                    await mostRecentWorkItem.Invoke(CancellationToken.None).ConfigureAwait(false);
                }, CancellationToken.None);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _throttleCancellationTokenSource.Cancel();
    }
}