namespace Luthetus.Common.Tests.Basis.Reactives.Models;

public class ThrottleEvent<T> : IThrottleEvent where T : notnull
{
    public ThrottleEvent(
        string id,
        TimeSpan throttleTimeSpan,
        T item,
        Func<IThrottleEvent, CancellationToken, Task> workItem,
        Func<(IThrottleEvent OldEvent, IThrottleEvent RecentEvent), IThrottleEvent?>? consecutiveEntryFunc)
    {
        Id = id;
        ThrottleTimeSpan = throttleTimeSpan;
        Item = item;
        WorkItem = workItem;
        ConsecutiveEntryFunc = consecutiveEntryFunc;
    }

    public T Item { get; }

    public string Id { get; }
    public TimeSpan ThrottleTimeSpan { get; }
    public Func<IThrottleEvent, CancellationToken, Task> WorkItem { get; }
    public Func<(IThrottleEvent OldEvent, IThrottleEvent RecentEvent), IThrottleEvent?>? ConsecutiveEntryFunc { get; }

    public Type Type => typeof(T);
    object IThrottleEvent.Item => Item;
}
