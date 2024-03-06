using System.Runtime.CompilerServices;

namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ThrottleController
{
    private readonly object _lockSemaphoreSlim = new();
    private readonly object _lockThrottleEventQueue = new();
    private readonly ThrottleEventQueue _throttleEventQueue = new();
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    private CancellationTokenSource _throttleCancellationTokenSource = new();
    private ConfiguredTaskAwaitable _throttleDelayTask = Task.CompletedTask.ConfigureAwait(false);
    private ConfiguredTaskAwaitable _previousWorkItemTask = Task.CompletedTask.ConfigureAwait(false);
    private ConfiguredTaskAwaitable _dequeueAsyncTask = Task.CompletedTask.ConfigureAwait(false);

    public void EnqueueEvent(IThrottleEvent throttleEvent)
    {
        lock (_lockThrottleEventQueue)
        {
            _throttleEventQueue.Enqueue(throttleEvent);

            if (_dequeueAsyncTask.GetAwaiter().IsCompleted)
                _dequeueAsyncTask = Task.Run(DequeueAsync).ConfigureAwait(false);
        }
    }

    private async Task DequeueAsync()
    {
        lock (_lockSemaphoreSlim)
        {
            if (_semaphoreSlim.CurrentCount <= 0)
                return;
        }

        try
        {
            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);

            while (true)
            {
                await _throttleDelayTask;
                await _previousWorkItemTask;

                CancellationToken cancellationToken;
                IThrottleEvent? throttleEvent;

                lock (_lockThrottleEventQueue)
                {
                    throttleEvent = _throttleEventQueue.DequeueOrDefault();
                }

                _throttleCancellationTokenSource.Cancel();
                _throttleCancellationTokenSource = new();
                cancellationToken = _throttleCancellationTokenSource.Token;

                if (throttleEvent is not null)
                {
                    _throttleDelayTask = Task.Run(async () =>
                    {
                        await Task.Delay(throttleEvent.ThrottleTimeSpan).ConfigureAwait(false);
                    }).ConfigureAwait(false);

                    _previousWorkItemTask = Task.Run(async () =>
                    {
                        await throttleEvent.HandleEvent(CancellationToken.None).ConfigureAwait(false);
                    }).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            lock (_lockSemaphoreSlim)
            {
                _semaphoreSlim.Release();
            }

            lock (_lockThrottleEventQueue)
            {
                if (_throttleEventQueue.Count > 0)
                {
                    // The _dequeueAsyncTask is started when one enqueues a workitem.  Specifically, if the _dequeueAsyncTask is
                    // completed, a task is ran for it. Otherwise, just let the already running one continue.
                    //
                    // But, if the already running _dequeueAsyncTask is in outside of it's "consumer" while loop when the enqueue
                    // function checks. Then the enqueue function thinks the _dequeueAsyncTask will handle the work item, but in
                    // reality the _dequeueAsyncTask is finishing, and will NOT handle the work item.
                    //
                    // This could leave "hanging" UI events. Where, the event was enqueue'd but this timing issue results in the
                    // UI event not being handled, because there is no _dequeueAsyncTask to act as a "consumer" in a
                    // producer-consumer pattern.
                    //
                    // For this reason, the finishing _dequeueAsyncTask will overwrite itself, with a new Task.Run(DequeueAsync).
                    _dequeueAsyncTask = Task.Run(DequeueAsync).ConfigureAwait(false);
                }
            }
        }
    }
    
    public async Task StopAsync()
    {
        await _dequeueAsyncTask;
    }

    public void Dispose()
    {
        _throttleCancellationTokenSource.Cancel();
    }
}
