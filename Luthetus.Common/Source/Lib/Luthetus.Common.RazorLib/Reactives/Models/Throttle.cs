namespace Luthetus.Common.RazorLib.Reactives.Models;

public class Throttle : IThrottle
{
    private readonly object _syncRoot = new();
    private readonly Stack<Func<Task>> _workItemsStack = new();

    private CancellationTokenSource _throttleCancellationTokenSource = new();
    private Task _throttleDelayTask = Task.CompletedTask;

    public Throttle(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    public TimeSpan ThrottleTimeSpan { get; }

    public async Task FireAsync(Func<Task> workItem)
    {
        lock (_syncRoot)
        {
            _workItemsStack.Push(workItem);

            if (_workItemsStack.Count > 1)
                return;
        }

        var cancellationToken = _throttleCancellationTokenSource.Token;

        await _throttleDelayTask;

        lock (_syncRoot)
        {
            var mostRecentEventArgs = _workItemsStack.Pop();
            _workItemsStack.Clear();

            _throttleCancellationTokenSource.Cancel();
            _throttleCancellationTokenSource = new();

            _ = Task.Run(async () =>
            {
                await workItem.Invoke();
            });

            _throttleDelayTask = Task.Run(async () =>
            {
                await Task.Delay(ThrottleTimeSpan);
            });
        }
    }

    public void Dispose()
    {
        _throttleCancellationTokenSource.Cancel();
    }
}