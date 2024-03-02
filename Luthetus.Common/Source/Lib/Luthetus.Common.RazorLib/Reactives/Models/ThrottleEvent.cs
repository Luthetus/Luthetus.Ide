namespace Luthetus.Common.Tests.Basis.Reactives.Models;

public class ThrottleEvent<T> : IThrottleEvent
{
    public ThrottleEvent(
        string id,
        TimeSpan throttleTimeSpan,
        Func<CancellationToken, Task> workItem,
        Func<(IThrottleEvent RecentEvent, IThrottleEvent OldEvent), IThrottleEvent> consecutiveEntryFunc)
    {
        Id = id;
        ThrottleTimeSpan = throttleTimeSpan;
        WorkItem = workItem;
        ConsecutiveEntryFunc = consecutiveEntryFunc;
    }

    public string Id { get; }
    public TimeSpan ThrottleTimeSpan { get; }
    public Func<CancellationToken, Task> WorkItem { get; }
    public Func<(IThrottleEvent RecentEvent, IThrottleEvent OldEvent), IThrottleEvent> ConsecutiveEntryFunc { get; }

    public Type Type => typeof(T);
}
