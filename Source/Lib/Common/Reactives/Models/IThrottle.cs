namespace Luthetus.Common.RazorLib.Reactives.Models;

public interface IThrottle : IDisposable
{
    public static readonly TimeSpan DefaultThrottleTimeSpan = TimeSpan.FromMilliseconds(15);

    /// <summary>
    /// The default value is true.
    /// <br/><br/>
    /// With <see cref="ShouldWaitForPreviousWorkItemToComplete"/> set to false,
    /// the only thing limiting invocations is the throttle delay.
    /// <br/><br/>
    /// With <see cref="ShouldWaitForPreviousWorkItemToComplete"/> set to true,
    /// then invocations are limited by both: the throttle delay, and
    /// the previous work item having completed.
    /// </summary>
    public bool ShouldWaitForPreviousWorkItemToComplete { get; }

    /// <summary>
    /// The workItemsStack entries are Func(s) that take the
    /// <see cref="_throttleCancellationTokenSource"/>, and return a task.
    /// <br/><br/>
    /// When <see cref="Dispose"/> is invoked, and cancellation tokens
    /// that came from _throttleCancellationTokenSource should be cancelled.
    /// </summary>
    public void PushEvent(Func<CancellationToken, Task> workItem);
}