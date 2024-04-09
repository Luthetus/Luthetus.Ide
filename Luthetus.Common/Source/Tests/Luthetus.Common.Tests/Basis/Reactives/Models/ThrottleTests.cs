using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Common.Tests.Basis.Reactives.Models;

/// <summary>
/// <see cref="Throttle"/>
/// </summary>
public class ThrottleTests
{
    /// <summary>
    /// <see cref="Throttle(TimeSpan)"/>
    /// <br/>----<br/>
    /// <see cref="Throttle.ThrottleTimeSpan"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var timeSpan = TimeSpan.FromMilliseconds(500);

        var throttle = new Throttle(timeSpan);

        Assert.Equal(timeSpan, throttle.ThrottleTimeSpan);
    }

    /// <summary>
    /// <see cref="Throttle.FireAsync(Func{CancellationToken, Task})"/>
    /// </summary>
    [Fact]
    public void FireAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="Throttle.Dispose()"/>
    /// </summary>
    [Fact]
    public Task DisposeAsync()
    {
        throw new NotImplementedException();

        var throttle = new Throttle(TimeSpan.FromMilliseconds(1_000));

        var counter = 0;

        throttle.PushEvent(throttleCancellationToken =>
        {
            counter++;
            return Task.CompletedTask;
        });

        Assert.Equal(1, counter);

        throttle.PushEvent(throttleCancellationToken =>
        {
            throttle.Dispose();

            if (throttleCancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            // Cancel the task prior to this incrementation
            counter++;
            return Task.CompletedTask;
        });

        // The second incrementation was cancelled by invoking 'Dispose()'
        Assert.Equal(1, counter);
    }
}