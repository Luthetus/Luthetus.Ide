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
        var taskKey = Key<IBackgroundTask>.NewKey();
        var taskQueueKey = Key<IBackgroundTaskQueue>.NewKey();
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
    /// <see cref="BackgroundTask.InvokeWorkItem(CancellationToken)"/>
    /// <br/>----<br/>
    /// <see cref="BackgroundTask.WorkProgress"/>
    /// </summary>
    [Fact]
    public async Task InvokeWorkItem()
    {
        var number = 0;
        Assert.Equal(0, number);

        var backgroundTaskKey = Key<IBackgroundTask>.NewKey();

        var backgroundTask = new BackgroundTask(
            backgroundTaskKey,
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Abc",
            () =>
            {
                // number += 1; from the task.
                number++;
                return Task.CompletedTask;
            });

        await backgroundTask.HandleEvent(CancellationToken.None);

        Assert.Equal(1, number);
    }
}