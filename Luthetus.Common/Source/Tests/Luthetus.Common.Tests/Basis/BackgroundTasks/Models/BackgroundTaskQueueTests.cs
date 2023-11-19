using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

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
    /// <br/>----<br/>
    /// <see cref="BackgroundTaskQueue.ExecutingBackgroundTaskChanged"/>
    /// </summary>
    [Fact]
    public void ExecutingBackgroundTask()
    {
        InitializeBackgroundTaskQueueTests(
            out var backgroundTaskService,
            out var queue,
            out _);

        Assert.Null(queue.ExecutingBackgroundTask);

        var number = 0;
        Assert.Equal(0, number);

        var backgroundTaskKey = Key<BackgroundTask>.NewKey();

        // number += 2; from the event.
        // Set executing to the task is +1, then set the executing to null is another +1
        void OnExecutingBackgroundTaskChanged() 
        {
            number++;
        }

        queue.ExecutingBackgroundTaskChanged += OnExecutingBackgroundTaskChanged;

        var backgroundTask = new BackgroundTask(
            backgroundTaskKey,
            queue.Key,
            "Abc",
            () =>
            {
                Assert.NotNull(queue.ExecutingBackgroundTask);
                Assert.Equal(backgroundTaskKey, queue.ExecutingBackgroundTask!.BackgroundTaskKey);

                // number += 1; from the task.
                number++;

                return Task.CompletedTask;
            });
        
        backgroundTaskService.Enqueue(backgroundTask);

        Assert.Equal(3, number);
        Assert.Null(queue.ExecutingBackgroundTask);
        
        queue.ExecutingBackgroundTaskChanged -= OnExecutingBackgroundTaskChanged;
    }

    private void InitializeBackgroundTaskQueueTests(
        out IBackgroundTaskService backgroundTaskService,
        out BackgroundTaskQueue continuousQueue,
        out BackgroundTaskQueue blockingQueue)
    {
        var services = new ServiceCollection()
            .AddScoped<IBackgroundTaskService>(_ => new BackgroundTaskServiceSynchronous())
            .AddFluxor(options => options.ScanAssemblies(typeof(IBackgroundTaskService).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        backgroundTaskService = serviceProvider.GetRequiredService<IBackgroundTaskService>();

        continuousQueue = new BackgroundTaskQueue(
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(continuousQueue);

        blockingQueue = new BackgroundTaskQueue(
            BlockingBackgroundTaskWorker.GetQueueKey(),
            BlockingBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(blockingQueue);
    }
}
