namespace Luthetus.Common.Tests.Basis.Reactives.Models;

public interface IThrottleEvent
{
    string Id { get; }
    TimeSpan ThrottleTimeSpan { get; }
    object Item { get; }
    Func<IThrottleEvent, CancellationToken, Task> WorkItem { get; }
    Func<(IThrottleEvent OldEvent, IThrottleEvent RecentEvent), IThrottleEvent?>? ConsecutiveEntryFunc { get; }
    Type Type { get; }
}
