using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskTests
{
    [Fact]
    public void Constructor()
    {
        /*
        public BackgroundTask(
            Key<BackgroundTask> backgroundTaskKey, Key<BackgroundTaskQueue> queueKey, string name, Func<Task> runFunc)
         */

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

        // [Fact]
        // public void BackgroundTaskKey()
        Assert.Equal(taskKey, backgroundTask.BackgroundTaskKey);

        // [Fact]
        // public void QueueKey()
        Assert.Equal(taskQueueKey, backgroundTask.QueueKey);

        // [Fact]
        // public void Name()
        Assert.Equal(name, backgroundTask.Name);
    }

    [Fact]
    public void WorkProgress()
    {
        /*
        public Task? WorkProgress { get; private set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void InvokeWorkItem()
    {
        /*
        public Task InvokeWorkItem(CancellationToken cancellationToken)
         */

        throw new NotImplementedException();
    }
}