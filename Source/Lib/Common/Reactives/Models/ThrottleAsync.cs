namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ThrottleAsync
{
    private readonly SemaphoreSlim _workItemsStackSemaphore = new(1, 1);
    private readonly Stack<Func<CancellationToken, Task>> _workItemsStack = new();
    private readonly SemaphoreSlim _activeTasksSemaphore = new(1, 1);

    private CancellationTokenSource _throttleCancellationTokenSource = new();
    private Task _activeThrottleDelayTask = Task.CompletedTask;
    private Task _activeTask = Task.CompletedTask;

    public bool ShouldWaitForPreviousWorkItemToComplete { get; } = true;
    public bool IsStoppingFurtherPushes { get; private set; }

    public ThrottleAsync(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    /// <summary>
    /// The default value for <see cref="ShouldWaitForPreviousWorkItemToComplete"/> is true
    /// </summary>
    public ThrottleAsync(TimeSpan throttleTimeSpan, bool shouldWaitForPreviousWorkItemToComplete)
    {
        ThrottleTimeSpan = throttleTimeSpan;
        ShouldWaitForPreviousWorkItemToComplete = shouldWaitForPreviousWorkItemToComplete;
    }

    public TimeSpan ThrottleTimeSpan { get; }

    public async Task PushEvent(Func<CancellationToken, Task> workItem)
    {
        // Push workItem onto stack
        try
        {
            await _workItemsStackSemaphore.WaitAsync().ConfigureAwait(false);

            _workItemsStack.Push(workItem);
            if (_workItemsStack.Count > 1)
                return;
        }
        finally
        {
            _workItemsStackSemaphore.Release();
        }

        try
        {
            await _activeTasksSemaphore.WaitAsync().ConfigureAwait(false);

            var localTask = _activeTask;
            var localThrottleDelayTask = _activeThrottleDelayTask;

            try
            {
                if (ShouldWaitForPreviousWorkItemToComplete)
                    await localTask.ConfigureAwait(false);

                await localThrottleDelayTask.ConfigureAwait(false);

                await _workItemsStackSemaphore.WaitAsync().ConfigureAwait(false);

                var newWorkItem = _workItemsStack.Pop();
                _workItemsStack.Clear();

                _activeThrottleDelayTask = Task.Run(() => Task.Delay(ThrottleTimeSpan));

                _activeTask = newWorkItem.Invoke(CancellationToken.None);
                await _activeTask.ConfigureAwait(false);
            }
            finally
            {
                _workItemsStackSemaphore.Release();
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////

            //var taskWrapper = Task.Run(async () =>
            //{
            //    try
            //    {
            //        if (ShouldWaitForPreviousWorkItemToComplete)
            //            await localTask.ConfigureAwait(false);

            //        await localThrottleDelayTask.ConfigureAwait(false);

            //        await _workItemsStackSemaphore.WaitAsync().ConfigureAwait(false);

            //        var newWorkItem = _workItemsStack.Pop();
            //        _workItemsStack.Clear();

            //        _activeThrottleDelayTask = Task.Run(() => Task.Delay(ThrottleTimeSpan));

            //        _activeTask = newWorkItem.Invoke(CancellationToken.None);
            //        await _activeTask.ConfigureAwait(false);
            //    }
            //    finally
            //    {
            //        _workItemsStackSemaphore.Release();
            //    }
            //});

            //if (ShouldWaitForPreviousWorkItemToComplete && localTask.IsCompleted &&
            //    localThrottleDelayTask.IsCompleted)
            //{
            //    await taskWrapper.ConfigureAwait(false);
            //}
        }
        finally
        {
            _activeTasksSemaphore.Release();
        }
    }

    public async Task StopFurtherPushes()
    {
        try
        {
            await _workItemsStackSemaphore.WaitAsync().ConfigureAwait(false);
            IsStoppingFurtherPushes = true;
        }
        finally
        {
            _workItemsStackSemaphore.Release();
        }
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
            if (_workItemsStack.Count == 0)
                break;

            await Task.Delay(pollingTimeSpan.Value);
        }

        await _activeTask;
    }

    public void Dispose()
    {
        _throttleCancellationTokenSource.Cancel();
    }
}