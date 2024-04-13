using System.Runtime.CompilerServices;

namespace Luthetus.Common.RazorLib.Reactives.Models;

public class Throttle : IThrottle
{
    private readonly object _lockWorkItemsStack = new();
    private readonly Stack<Func<CancellationToken, Task>> _workItemsStack = new();

    private CancellationTokenSource _throttleCancellationTokenSource = new();
    private ConfiguredTaskAwaitable _throttleDelayTask = Task.CompletedTask.ConfigureAwait(false);
    private ConfiguredTaskAwaitable _previousWorkItemTask = Task.CompletedTask.ConfigureAwait(false);

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

    public void PushEvent(Func<CancellationToken, Task> workItem)
    {
        lock (_lockWorkItemsStack)
        {
            _workItemsStack.Push(workItem);

            if (_workItemsStack.Count > 1)
                return;

            _ = Task.Run(async () =>
            {
                await _throttleDelayTask;

                if (ShouldWaitForPreviousWorkItemToComplete)
                    await _previousWorkItemTask;

                CancellationToken cancellationToken;
                Func<CancellationToken, Task> mostRecentWorkItem;

                lock (_lockWorkItemsStack)
                {
                    mostRecentWorkItem = _workItemsStack.Pop();
                    _workItemsStack.Clear();

                    _throttleCancellationTokenSource.Cancel();
                    _throttleCancellationTokenSource = new();
                    cancellationToken = _throttleCancellationTokenSource.Token;

                    _throttleDelayTask = Task.Run(async () =>
                    {
                        await Task.Delay(ThrottleTimeSpan).ConfigureAwait(false);
                    }).ConfigureAwait(false);

                    _previousWorkItemTask = Task.Run(async () =>
                    {
                        await mostRecentWorkItem.Invoke(CancellationToken.None).ConfigureAwait(false);
                    }).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _throttleCancellationTokenSource.Cancel();
    }
}