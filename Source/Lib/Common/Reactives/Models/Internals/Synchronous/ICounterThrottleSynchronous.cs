namespace Luthetus.Common.RazorLib.Reactives.Models.Internals.Synchronous;

public interface ICounterThrottleSynchronous : ICounterThrottleData
{
    public object WorkItemLock { get; }

    public void PushEvent(
        Func<Task> workItem,
        Func<double, Task>? progressFunc = null,
        CancellationToken delayCancellationToken = default);
}
