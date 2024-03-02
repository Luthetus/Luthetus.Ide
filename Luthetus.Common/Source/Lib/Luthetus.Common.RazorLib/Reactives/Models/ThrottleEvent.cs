namespace Luthetus.Common.Tests.Basis.Reactives.Models;

public class ThrottleEvent<T> : IThrottleEvent
{
    public ThrottleEvent(
        string id,
        TimeSpan throttleTimeSpan,
        Func<CancellationToken, Task> workItem,
        Func<(IThrottleEvent OldEvent, IThrottleEvent RecentEvent), IThrottleEvent> consecutiveEntryFunc)
    {
        Id = id;
        ThrottleTimeSpan = throttleTimeSpan;
        WorkItem = workItem;
        ConsecutiveEntryFunc = consecutiveEntryFunc;
    }

    public string Id { get; }
    public TimeSpan ThrottleTimeSpan { get; }
    public Func<CancellationToken, Task> WorkItem { get; }
    public Func<(IThrottleEvent OldEvent, IThrottleEvent RecentEvent), IThrottleEvent> ConsecutiveEntryFunc { get; }

    public Type Type => typeof(T);
}
