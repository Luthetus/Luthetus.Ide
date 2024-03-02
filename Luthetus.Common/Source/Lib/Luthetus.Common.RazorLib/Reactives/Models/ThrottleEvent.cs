namespace Luthetus.Common.Tests.Basis.Reactives.Models;

public class ThrottleEvent<T> : IThrottleEvent
{
    public ThrottleEvent(
        string id,
        TimeSpan throttleTimeSpan,
        Func<CancellationToken, Task> workItem,
        Func<IThrottleEvent, IThrottleEvent, IThrottleEvent> consecutiveEntryFunc)
    {
        Id = id;
        ThrottleTimeSpan = throttleTimeSpan;
        WorkItem = workItem;
        ConsecutiveEntryFunc = consecutiveEntryFunc;
    }

    public string Id { get; }
    public TimeSpan ThrottleTimeSpan { get; }
    public Func<CancellationToken, Task> WorkItem { get; }
    public Func<IThrottleEvent, IThrottleEvent, IThrottleEvent> ConsecutiveEntryFunc { get; }

    public Type Type => typeof(T);
}
