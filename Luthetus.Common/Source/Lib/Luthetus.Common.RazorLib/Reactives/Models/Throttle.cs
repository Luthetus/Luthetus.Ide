namespace Luthetus.Common.RazorLib.Reactives.Models;

public class Throttle : IThrottle
{
    private readonly object _syncRoot = new();
    private readonly Stack<Func<CancellationToken, Task>> _workItemsStack = new();
    
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
        lock (_syncRoot)
        {
            _workItemsStack.Push(workItem);

            if (_workItemsStack.Count > 1)
                return;
        }

        _ = Task.Run(async () =>
        {
            await _throttleDelayTask.ConfigureAwait(false);

            if (ShouldWaitForPreviousWorkItemToComplete)
                await _previousWorkItemTask.ConfigureAwait(false);

            Func<CancellationToken, Task> mostRecentWorkItem;
            CancellationToken cancellationToken;

            lock (_syncRoot)
            {
                mostRecentWorkItem = _workItemsStack.Pop();
                _workItemsStack.Clear();

                _throttleCancellationTokenSource.Cancel();
                _throttleCancellationTokenSource = new();

                cancellationToken = _throttleCancellationTokenSource.Token;

                _throttleDelayTask = Task.Run(async () =>
                {
                    await Task.Delay(ThrottleTimeSpan).ConfigureAwait(false);
                }, cancellationToken);
            }

            _previousWorkItemTask = mostRecentWorkItem.Invoke(cancellationToken);
            await _previousWorkItemTask.ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public void Dispose()
    {
        _throttleCancellationTokenSource.Cancel();
    }
}