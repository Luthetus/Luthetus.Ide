namespace Luthetus.Common.RazorLib.Reactives.Models.Internals.Async;

public interface ICounterThrottleAsync : ICounterThrottleData
{
    public SemaphoreSlim WorkItemSemaphore { get; }

    public Task PushEvent(
        Func<Task> workItem,
        Func<double, Task>? progressFunc = null,
        CancellationToken delayCancellationToken = default);
}
