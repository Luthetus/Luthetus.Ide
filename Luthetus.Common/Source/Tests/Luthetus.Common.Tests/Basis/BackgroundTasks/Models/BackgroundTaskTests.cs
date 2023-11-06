using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="BackgroundTask"/>
/// </summary>
public class BackgroundTaskTests
{
    /// <summary>
    /// <see cref="BackgroundTask(Key{BackgroundTask}, Key{BackgroundTaskQueue}, string, Func{Task})"/>
    /// <br/>----<br/>
    /// <see cref="BackgroundTask.BackgroundTaskKey"/>
    /// <see cref="BackgroundTask.QueueKey"/>
    /// <see cref="BackgroundTask.Name"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
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

        Assert.Equal(taskKey, backgroundTask.BackgroundTaskKey);
        Assert.Equal(taskQueueKey, backgroundTask.QueueKey);
        Assert.Equal(name, backgroundTask.Name);
    }

    /// <summary>
    /// <see cref="BackgroundTask.WorkProgress"/>
    /// </summary>
    [Fact]
    public void WorkProgress()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTask.InvokeWorkItem(CancellationToken)"/>
    /// </summary>
    [Fact]
    public void InvokeWorkItem()
    {
        throw new NotImplementedException();
    }
}