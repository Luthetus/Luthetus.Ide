namespace Luthetus.Common.RazorLib.Reactives.Models.Internals;

public class CounterThrottleAsync : ICounterThrottleAsync
{
    public readonly SemaphoreSlim _workItemSemaphore = new(1, 1);
    public readonly Stack<Func<Task>> _workItemStack = new();

    public Task _delayTask = Task.CompletedTask;
    public Task _workItemTask = Task.CompletedTask;
    public ExecutionKind _executionKind = ExecutionKind.Await;

    public TimeSpan ThrottleTimeSpan { get; }

    public async Task SetExecuteKind(ExecutionKind executionKind)
    {
        try
        {
            await _workItemSemaphore.WaitAsync().ConfigureAwait(false);

            _executionKind = executionKind;
        }
        finally
        {
            _workItemSemaphore.Release();
        }
    }

    public async Task PushEvent(Func<Task> workItem)
    {
        var localExecutionKind = _executionKind;

        await Push(workItem);

        switch (localExecutionKind)
        {
            case ExecutionKind.Await:
                await Execute_Await();
                break;
            case ExecutionKind.TaskRun:
                await Execute_TaskRun();
                break;
            default:
                throw new NotImplementedException($"The {nameof(ExecutionKind)}: '{localExecutionKind}' was not recognized.");
        }
    }

    public async Task Execute_Await()
    {
        Func<Task> popWorkItem;
        try
        {
            await _workItemSemaphore.WaitAsync().ConfigureAwait(false);

            popWorkItem = _workItemStack.Pop();
            _workItemStack.Clear();
        }
        finally
        {
            _workItemSemaphore.Release();
        }

        _delayTask = Task.Delay(ThrottleTimeSpan);
        _workItemTask = popWorkItem.Invoke();

        await Task.WhenAll(_delayTask, _workItemTask);
    }

    public async Task Execute_TaskRun()
    {
        Func<Task> popWorkItem;
        try
        {
            await _workItemSemaphore.WaitAsync().ConfigureAwait(false);

            popWorkItem = _workItemStack.Pop();
            _workItemStack.Clear();
        }
        finally
        {
            _workItemSemaphore.Release();
        }

        _delayTask = Task.Run(async () => await Task.Delay(ThrottleTimeSpan));
        _workItemTask = Task.Run(async () => await popWorkItem.Invoke());
    }

    /// <summary>
    /// The goal of this class is to visualize various attempts of implementing
    /// a throttle.<br/><br/>
    /// 
    /// Its presumed that all the versions will have this code be equivalent,
    /// so moving it into its own method helps for readability.
    /// </summary>
    private async Task Push(Func<Task> workItem)
    {
        try
        {
            await _workItemSemaphore.WaitAsync().ConfigureAwait(false);

            _workItemStack.Push(workItem);
            if (_workItemStack.Count > 1)
                return;
        }
        finally
        {
            _workItemSemaphore.Release();
        }
    }

    public enum ExecutionKind
    {
        Await,
        TaskRun,
    }
}
