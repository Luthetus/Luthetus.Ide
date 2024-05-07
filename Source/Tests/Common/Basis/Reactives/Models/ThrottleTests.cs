using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Common.Tests.Basis.Reactives.Models;

/// <summary>
/// <see cref="Throttle"/>
/// </summary>
public class ThrottleTests
{
    /// <summary>
    /// <see cref="Throttle.ShouldWaitForPreviousWorkItemToComplete"/>
    /// </summary>
    [Fact]
    public void ShouldWaitForPreviousWorkItemToComplete()
    {
        //public bool ShouldWaitForPreviousWorkItemToComplete { get; } = true;
    }

    /// <summary>
    /// <see cref="Throttle(TimeSpan)"/>
    /// </summary>
    [Fact]
    public void Constructor_TimeSpan()
    {
        //public Throttle(TimeSpan throttleTimeSpan)
    }

    /// <summary>
    /// <see cref="Throttle(TimeSpan, bool)"/>
    /// </summary>
    [Fact]
    public void Constructor_TimeSpan_bool()
    {
        //public Throttle(TimeSpan throttleTimeSpan, bool shouldWaitForPreviousWorkItemToComplete)
    }

    /// <summary>
    /// <see cref="Throttle.ThrottleTimeSpan"/>
    /// </summary>
    [Fact]
    public void ThrottleTimeSpan()
    {
        //public TimeSpan ThrottleTimeSpan { get; }
    }

    /// <summary>
    /// <see cref="Throttle.PushEvent(Func{CancellationToken, Task})"/>
    /// </summary>
    [Fact]
    public void PushEvent()
    {
        //public void PushEvent(Func<CancellationToken, Task> workItem)
    }

    /// <summary>
    /// <see cref="Throttle.Dispose()"/>
    /// </summary>
    [Fact]
    public void Dispose()
    {
        //public void Dispose()
    }
}