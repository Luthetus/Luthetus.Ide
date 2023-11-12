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
        
        var queue = new BackgroundTaskQueue(key, displayName);

        Assert.Equal(key, queue.Key);
        Assert.Equal(displayName, queue.DisplayName);

        var taskKey = Key<BackgroundTask>.NewKey();
        var taskQueueKey = Key<BackgroundTaskQueue>.NewKey();
        var name = "Write \"Hello World!\" to the console";

        var backgroundTask = new BackgroundTask(
            taskKey,
            taskQueueKey,
            name,
            () =>
            {
                Console.WriteLine("Hello World!");
                return Task.CompletedTask;
            });
        
        Assert.Empty(queue.BackgroundTasks);
        Assert.Equal(0, queue.CountOfBackgroundTasks);

        queue.BackgroundTasks.Enqueue(backgroundTask);
        
        Assert.NotEmpty(queue.BackgroundTasks);
        Assert.Equal(1, queue.CountOfBackgroundTasks);
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
    /// <see cref="BackgroundTaskQueue.ExecutingBackgroundTaskChanged"/>
    /// </summary>
    [Fact]
    public void ExecutingBackgroundTaskChanged()
    {
        throw new NotImplementedException();
    }
}
