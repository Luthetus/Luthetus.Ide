namespace Luthetus.Common.RazorLib.Reactives.Models.Internals;

public class CounterThrottleAsync : ICounterThrottleAsync
{
    public readonly SemaphoreSlim _workItemSemaphore = new(1, 1);
    public readonly Stack<Func<Task>> _workItemStack = new();

    public TimeSpan ThrottleTimeSpan { get; }

    public async Task PushEvent(Func<Task> workItem)
    {
        // Push workItem onto stack
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
}
