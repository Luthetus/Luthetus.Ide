namespace Luthetus.Common.RazorLib.Reactives.Models;

public interface IThrottle : IDisposable
{
    public static readonly TimeSpan DefaultThrottleTimeSpan = TimeSpan.FromMilliseconds(15);

    /// <summary>
    /// The cancellation logic should be made internal to the workItem Func itself.
    /// </summary>
    public Task FireAsync(Func<Task> workItem);
}