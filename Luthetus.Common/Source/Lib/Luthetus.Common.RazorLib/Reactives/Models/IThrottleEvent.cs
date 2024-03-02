namespace Luthetus.Common.Tests.Basis.Reactives.Models;

public interface IThrottleEvent
{
    string Id { get; }
    TimeSpan ThrottleTimeSpan { get; }
    Func<CancellationToken, Task> WorkItem { get; }
    Func<(IThrottleEvent OldEvent, IThrottleEvent RecentEvent), IThrottleEvent>? ConsecutiveEntryFunc { get; }
    Type Type { get; }
}
