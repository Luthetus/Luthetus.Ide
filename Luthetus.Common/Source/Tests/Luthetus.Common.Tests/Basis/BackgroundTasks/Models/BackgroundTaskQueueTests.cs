using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="BackgroundTaskQueue"/>
/// </summary>
public class BackgroundTaskQueueTests
{
    /// <summary>
    /// <see cref="BackgroundTaskQueue(Key{BackgroundTaskQueue}, string)"/>
    /// <br/>----<br/>
    /// <see cref="BackgroundTaskQueue.Key"/>
    /// <see cref="BackgroundTaskQueue.DisplayName"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var key = Key<BackgroundTaskQueue>.NewKey();
        var displayName = "Continuous";

        var backgroundTaskQueue = new BackgroundTaskQueue(
            key,
            displayName);

        Assert.Equal(key, backgroundTaskQueue.Key);
        Assert.Equal(displayName, backgroundTaskQueue.DisplayName);
    }

    /// <summary>
    /// <see cref="BackgroundTaskQueue.BackgroundTasks"/>
    /// </summary>
    [Fact]
    public void BackgroundTasks()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskQueue.WorkItemsQueueSemaphoreSlim"/>
    /// </summary>
    [Fact]
    public void WorkItemsQueueSemaphoreSlim()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskQueue.ExecutingBackgroundTask"/>
    /// </summary>
    [Fact]
    public void ExecutingBackgroundTask()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskQueue.CountOfBackgroundTasks"/>
    /// </summary>
    [Fact]
    public void CountOfBackgroundTasks()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskQueue.ExecutingBackgroundTaskChanged"/>
    /// </summary>
    [Fact]
    public void ExecutingBackgroundTaskChanged()
    {
        throw new NotImplementedException();
    }
}
