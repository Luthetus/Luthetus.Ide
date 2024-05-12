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

    /// <summary>
    /// TODO: I'm tired and going to try and type out my final thoughts before I go to sleep...
    ///       ...There might be some trickery going on here when changing from
    ///       Blazor-ServerSide to Blazor-WebAssembly.
    ///       |
    ///       As of this comment, ServerSide is running incredibly smooth,
    ///       but I run the site with WebAssembly then the UI seems to frequently freeze up
    ///       when accepting a flow of user input (i.e. holding 'ArrowRight' to move in the text editor).
    ///       |
    ///       The idea is: in this method, there is a lock. And this lock is messing WebAssembly up.
    ///       Within the lock, I have a '_ = Task.Run(...)'.
    ///       I execute it in a "fire and forget" manner from within the lock.
    ///       |
    ///       Is it possible that WebAssembly running single threaded could result
    ///       in the "fire and forget" task being executed from within the lock?
    ///       |
    ///       This being opposed to: enter lock, fire and forget, leave lock, execute fire and forget task.
    ///       |
    ///       I'm not sure if that even makes sense. If its the same thread then no deadlock can occur here.
    ///       |
    ///       That being said, this method is invoked the UI Synchronization Context.
    ///       Perhaps the lock needs to be changed to some alternative thread safety implementation.
    ///       |
    ///       Now that I say it I'm not even sure if this method is used anywhere I need to
    ///       go to sleep then look at this tomorrow.
    /// </summary>
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