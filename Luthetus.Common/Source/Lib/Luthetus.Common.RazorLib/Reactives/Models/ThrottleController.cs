/*
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using System.Runtime.CompilerServices;

namespace Luthetus.Common.RazorLib.Reactives.Models;

/// <summary>
/// Goal: Change BackgroundTask to act similarly to IThrottleEvent #Step 300 (2024-03-11)
/// -------------------------------------------------------------------------------------
/// I want to delete the IThrottleEvent type, and replace all references to it with
/// <see cref"IBackgroundTask"/>
/// </summary>
public class ThrottleController
{
    private readonly object _lockSemaphoreSlim = new();
    private readonly object _lockThrottleEventQueue = new();
    private readonly ThrottleEventQueue _throttleEventQueue = new();
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    private CancellationTokenSource _throttleCancellationTokenSource = new();
    private ConfiguredTaskAwaitable _dequeueAsyncTask = Task.CompletedTask.ConfigureAwait(false);

    public void EnqueueEvent(IBackgroundTask throttleEvent)
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
        
        var success = await _semaphoreSlim.WaitAsync(0).ConfigureAwait(false);

        if (!success)
            return;

        try
        {
            while (true)
            {
                CancellationToken cancellationToken;
                IBackgroundTask? throttleEvent;

                lock (_lockThrottleEventQueue)
                {
                    throttleEvent = _throttleEventQueue.DequeueOrDefault();
                }

                _throttleCancellationTokenSource.Cancel();
                _throttleCancellationTokenSource = new();
                cancellationToken = _throttleCancellationTokenSource.Token;

                if (throttleEvent is not null)
                {
                    await Task.WhenAll(
                        throttleEvent.HandleEvent(CancellationToken.None),
                        Task.Delay(throttleEvent.ThrottleTimeSpan));
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
*/