using System.Runtime.CompilerServices;

namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ThrottleController
{
    private readonly object _lockSemaphoreSlim = new();
    private readonly object _lockThrottleEventQueue = new();
    private readonly Queue<IThrottleEvent> _throttleEventQueue = new();
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
                IThrottleEvent? oldEvent;

                // A lock is not needed for dequeueing from _throttleEventQueue
                // because the only dequeues are happening here in a SemaphoreSlim.
                if (_throttleEventQueue.TryDequeue(out oldEvent) && oldEvent is not null)
                {
                    // In order to avoid infinitely writing, then dequeueing over and over.
                    // Get the current count so there is a maximum amount of times this inner loop will run
                    // when combining consecutive entries.
                    int captureQueueCount = _throttleEventQueue.Count;

                    for (int i = 0; i < captureQueueCount; i++)
                    {
                        if (oldEvent.ConsecutiveEntryFunc is not null &&
                            _throttleEventQueue.TryPeek(out var recentEvent) && recentEvent is not null &&
                                oldEvent.Id == recentEvent.Id)
                        {
                            var consecutiveResult = oldEvent.ConsecutiveEntryFunc.Invoke((oldEvent, recentEvent));

                            if (consecutiveResult is null)
                            {
                                break;
                            }
                            else
                            {
                                // Because the 'ConsecutiveEntryFunc' function successfully merged
                                // the two work items, then dequeue the recentEvent since it will be handled.
                                _throttleEventQueue.TryDequeue(out recentEvent);
                                oldEvent = consecutiveResult;
                            }
                        }
                    }

                    _throttleCancellationTokenSource.Cancel();
                    _throttleCancellationTokenSource = new();
                    cancellationToken = _throttleCancellationTokenSource.Token;
                }
                else
                {
                    break;
                }

                if (oldEvent is not null)
                {
                    _throttleDelayTask = Task.Run(async () =>
                    {
                        await Task.Delay(oldEvent.ThrottleTimeSpan).ConfigureAwait(false);
                    }).ConfigureAwait(false);

                    _previousWorkItemTask = Task.Run(async () =>
                    {
                        await oldEvent.WorkItem.Invoke(oldEvent, CancellationToken.None).ConfigureAwait(false);
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
                    // The _dequeueAsyncTask is started when one enqueues a workitem.
                    //
                    // Specifically, if the _dequeueAsyncTask is completed, a task is ran for it.
                    // otherwise, just let the already running one continue.
                    //
                    // But, if the already running _dequeueAsyncTask is in outside of it's
                    // "consumer" while loop when the enqueue function checks.
                    //
                    // Then the enqueue function thinks the _dequeueAsyncTask will handle the work item,
                    // but in reality the _dequeueAsyncTask is finishing, and will NOT handle the work item.
                    //
                    // This could leave "hanging" UI events. Where, the event was enqueue'd
                    // but this timing issue results in the UI event not being handled, because there
                    // is no _dequeueAsyncTask to act as a "consumer" in a producer-consumer pattern.
                    //
                    // For this reason, the finishing _dequeueAsyncTask will overwrite itself,
                    // with a new Task.Run(DequeueAsync).
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
