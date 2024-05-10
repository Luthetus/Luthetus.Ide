namespace Luthetus.Common.RazorLib.Reactives.Models.Internals;

public class CounterThrottleAsync : ICounterThrottleAsync
{
    public readonly SemaphoreSlim _workItemSemaphore = new(1, 1);
    public readonly Stack<Func<Task>> _workItemStack = new();
    private readonly object _idLock = new();

    public CounterThrottleAsync(TimeSpan throttleTimeSpan)
    {
        ThrottleTimeSpan = throttleTimeSpan;
    }

    public Task _delayTask = Task.CompletedTask;
    public Task _workItemTask = Task.CompletedTask;
    public ExecutionKind _executionKind = ExecutionKind.Await;

    public TimeSpan ThrottleTimeSpan { get; }
    public SynchronizationContext? PushEventStart_SynchronizationContext { get; private set; }
    public SynchronizationContext? PushEventEnd_SynchronizationContext { get; private set; }
    public Thread? PushEventStart_Thread { get; private set; }
    public Thread? PushEventEnd_Thread { get; private set; }
    public (int Id, DateTime DateTime) PushEventStart_DateTimeTuple { get; private set; } = (-1, DateTime.MinValue);
    public (int Id, DateTime DateTime) PushEventEnd_DateTimeTuple { get; private set; } = (-1, DateTime.MinValue);

    private int _getId;

    public async Task PushEvent(Func<Task> workItem)
    {
        int id;
        lock (_idLock)
        {
            // TODO: I want the _id to be unique, but I also wonder...
            //       ...if adding this 'lock' logic has any effect
            //       on all the async/thread things I'm looking into.
            id = ++_getId;
        }

        PushEventStart_SynchronizationContext = SynchronizationContext.Current;
        PushEventStart_Thread = Thread.CurrentThread;
        PushEventStart_DateTimeTuple = (id, DateTime.UtcNow);

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
            case ExecutionKind.Mix:
                await Execute_Mix();
                break;
            default:
                throw new NotImplementedException($"The {nameof(ExecutionKind)}: '{localExecutionKind}' was not recognized.");
        }

        PushEventEnd_Thread = Thread.CurrentThread;
        PushEventEnd_SynchronizationContext = SynchronizationContext.Current;
        PushEventEnd_DateTimeTuple = (id, DateTime.UtcNow);
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
            await _workItemSemaphore.WaitAsync();

            _workItemStack.Push(workItem);
            if (_workItemStack.Count > 1)
                return;
        }
        finally
        {
            _workItemSemaphore.Release();
        }
    }

    public async Task Execute_Await()
    {
        await _delayTask;

        Func<Task> popWorkItem;
        try
        {
            await _workItemSemaphore.WaitAsync();

            if (_workItemStack.Count == 0)
                return;

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
            await _workItemSemaphore.WaitAsync();

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

    public async Task Execute_Mix()
    {
        Func<Task> popWorkItem;
        try
        {
            await _workItemSemaphore.WaitAsync();

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

    public async Task SetExecuteKind(ExecutionKind executionKind)
    {
        try
        {
            await _workItemSemaphore.WaitAsync();

            _executionKind = executionKind;
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
        Mix,
    }
}
