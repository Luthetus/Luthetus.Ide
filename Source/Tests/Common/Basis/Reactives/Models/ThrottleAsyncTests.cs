using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Common.Tests.Basis.Reactives.Models;

/// <summary>
/// <see cref="ThrottleAsync"/>
/// </summary>
public class ThrottleAsyncTests
{
    /// <summary>
    /// <see cref="ThrottleAsync(TimeSpan)"/>
    /// <br/>----<br/>
    /// <see cref="ThrottleAsync.ShouldWaitForPreviousWorkItemToComplete"/>
    /// <see cref="ThrottleAsync.ThrottleTimeSpan"/>
    /// </summary>
    [Fact]
    public void Constructor_TimeSpan()
    {
        // Testing two distinct values to ensure the parameter didn't happen to be the default value.
        {
            var timeSpan = TimeSpan.FromMilliseconds(333);
            var throttle = new ThrottleAsync(timeSpan);
            Assert.Equal(timeSpan, throttle.ThrottleTimeSpan);
            Assert.True(throttle.ShouldWaitForPreviousWorkItemToComplete);
        }

        // Testing two distinct values to ensure the parameter didn't happen to be the default value.
        {
            var timeSpan = TimeSpan.FromMilliseconds(100);
            var throttle = new ThrottleAsync(timeSpan);
            Assert.Equal(timeSpan, throttle.ThrottleTimeSpan);
            Assert.True(throttle.ShouldWaitForPreviousWorkItemToComplete);
        }
    }

    /// <summary>
    /// <see cref="ThrottleAsync(TimeSpan, bool)"/>
    /// <br/>----<br/>
    /// <see cref="ThrottleAsync.ShouldWaitForPreviousWorkItemToComplete"/>
    /// <see cref="ThrottleAsync.ThrottleTimeSpan"/>
    /// </summary>
    [Fact]
    public void Constructor_TimeSpan_bool()
    {
        // true == ShouldWaitForPreviousWorkItemToComplete
        {
            // Testing two distinct values to ensure the parameter didn't happen to be the default value.
            {
                var timeSpan = TimeSpan.FromMilliseconds(333);
                var shouldWaitForPreviousWorkItemToComplete = true;
                var throttle = new ThrottleAsync(timeSpan, shouldWaitForPreviousWorkItemToComplete);
                Assert.Equal(timeSpan, throttle.ThrottleTimeSpan);
                Assert.True(throttle.ShouldWaitForPreviousWorkItemToComplete);
            }

            // Testing two distinct values to ensure the parameter didn't happen to be the default value.
            {
                var timeSpan = TimeSpan.FromMilliseconds(100);
                var shouldWaitForPreviousWorkItemToComplete = true;
                var throttle = new ThrottleAsync(timeSpan, shouldWaitForPreviousWorkItemToComplete);
                Assert.Equal(timeSpan, throttle.ThrottleTimeSpan);
                Assert.True(throttle.ShouldWaitForPreviousWorkItemToComplete);
            }
        }

        // false == ShouldWaitForPreviousWorkItemToComplete
        {
            // Testing two distinct values to ensure the parameter didn't happen to be the default value.
            {
                var timeSpan = TimeSpan.FromMilliseconds(333);
                var shouldWaitForPreviousWorkItemToComplete = false;
                var throttle = new ThrottleAsync(timeSpan, shouldWaitForPreviousWorkItemToComplete);
                Assert.Equal(timeSpan, throttle.ThrottleTimeSpan);
                Assert.False(throttle.ShouldWaitForPreviousWorkItemToComplete);
            }

            // Testing two distinct values to ensure the parameter didn't happen to be the default value.
            {
                var timeSpan = TimeSpan.FromMilliseconds(100);
                var shouldWaitForPreviousWorkItemToComplete = false;
                var throttle = new ThrottleAsync(timeSpan, shouldWaitForPreviousWorkItemToComplete);
                Assert.Equal(timeSpan, throttle.ThrottleTimeSpan);
                Assert.False(throttle.ShouldWaitForPreviousWorkItemToComplete);
            }
        }
    }

    /// <summary>
    /// <see cref="ThrottleAsync.PushEvent(Func{CancellationToken, Task})"/>
    /// </summary>
    [Fact]
    public async Task PushEvent_One()
    {
        var throttle = new ThrottleAsync(TimeSpan.FromMilliseconds(100));

        int i = 0;

        var workItem = new Func<CancellationToken, Task>(_ =>
        {
            i++;
            return Task.CompletedTask;
        });

        await throttle.PushEvent(workItem);

        await throttle.StopFurtherPushes();
        await throttle.UntilIsEmpty();

        Assert.Equal(1, i);
    }

    /// <summary>
    /// <see cref="ThrottleAsync.PushEvent(Func{CancellationToken, Task})"/>
    /// </summary>
    [Fact]
    public async Task PushEvent_Two()
    {
        var throttle = new ThrottleAsync(TimeSpan.FromMilliseconds(5_000));

        int i = 0;

        var workItem = new Func<CancellationToken, Task>(_ =>
        {
            i++;
            return Task.CompletedTask;
        });

        await throttle.PushEvent(workItem);
        await throttle.PushEvent(workItem);

        await throttle.StopFurtherPushes();
        await throttle.UntilIsEmpty();

        Assert.Equal(2, i);
    }
    
    /// <summary>
    /// <see cref="ThrottleAsync.PushEvent(Func{CancellationToken, Task})"/>
    /// </summary>
    [Fact]
    public async Task PushEvent_Three()
    {
        var throttle = new ThrottleAsync(TimeSpan.FromMilliseconds(5_000));

        int i = 0;

        var workItem = new Func<CancellationToken, Task>(_ =>
        {
            i++;
            return Task.CompletedTask;
        });

        await throttle.PushEvent(workItem);
        await throttle.PushEvent(workItem);
        await throttle.PushEvent(workItem);

        await throttle.StopFurtherPushes();
        await throttle.UntilIsEmpty();

        Assert.Equal(2, i);
    }
    
    /// <summary>
    /// <see cref="ThrottleAsync.PushEvent(Func{CancellationToken, Task})"/>
    /// </summary>
    [Fact]
    public async Task PushEvent_Four()
    {
        var throttle = new ThrottleAsync(TimeSpan.FromMilliseconds(5_000));

        int i = 0;

        var workItem = new Func<CancellationToken, Task>(_ =>
        {
            i++;
            return Task.CompletedTask;
        });

        await throttle.PushEvent(workItem);
        await throttle.PushEvent(workItem);
        await throttle.PushEvent(workItem);
        await throttle.PushEvent(workItem);

        await throttle.StopFurtherPushes();
        await throttle.UntilIsEmpty();

        Assert.Equal(2, i);
    }

    /// <summary>
    /// <see cref="ThrottleAsync.Dispose()"/>
    /// </summary>
    [Fact]
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}