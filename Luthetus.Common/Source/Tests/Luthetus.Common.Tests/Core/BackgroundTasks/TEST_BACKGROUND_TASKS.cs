using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Luthetus.Common.Tests.Core.BackgroundTasks;

public class TEST_BACKGROUND_TASKS
{
    [Fact]
    public async Task Async_BackgroundTasks_Test()
    {
        // -Create a local BackgroundTaskService instance
        // -Start the background task service as a fire and forget task
        // -Enqueue two background tasks
        // -Await the method for stopping the BackgroundTaskService
        // -Assert that both background tasks completed.

        var backgroundTaskService = new BackgroundTaskService();

        backgroundTaskService.RegisterQueue(ContinuousBackgroundTaskWorker.Queue);

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetRequiredService<ILoggerFactory>();

        var backgroundTaskWorker = new ContinuousBackgroundTaskWorker(
            ContinuousBackgroundTaskWorker.Queue.Key,
            backgroundTaskService,
            factory);

        var firstBackgroundTask = new BackgroundTask(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.Queue.Key,
            "firstTask",
            async () =>
            {
                for (var i = 0; i < 3; i++)
                {
                    await Task.Delay(33);
                }
            });

        backgroundTaskService.Enqueue(firstBackgroundTask);

        var secondBackgroundTask = new BackgroundTask(Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.Queue.Key,
            "secondTask",
            async () =>
            {
                for (var i = 0; i < 3; i++)
                {
                    await Task.Delay(33);
                }
            });

        backgroundTaskService.Enqueue(secondBackgroundTask);

        var startCts = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            await backgroundTaskWorker.StartAsync(startCts.Token);
        });

        var stopCts = new CancellationTokenSource();
        await backgroundTaskWorker.StopAsync(stopCts.Token);

        Assert.NotNull(firstBackgroundTask.WorkProgress);
        Assert.NotNull(secondBackgroundTask.WorkProgress);

        Assert.True(firstBackgroundTask.WorkProgress!.IsCompleted);
        Assert.True(secondBackgroundTask.WorkProgress!.IsCompleted);
    }

    [Fact]
    public void Sync_BackgroundTasks_Test()
    {
        var syncBackgroundTaskService = new BackgroundTaskServiceSynchronous();

        syncBackgroundTaskService.RegisterQueue(ContinuousBackgroundTaskWorker.Queue);

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetRequiredService<ILoggerFactory>();

        var firstBackgroundTask = new BackgroundTask(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.Queue.Key,
            "firstTask",
            async () =>
            {
                for (var i = 0; i < 3; i++)
                {
                    await Task.Delay(33);
                }
            });

        syncBackgroundTaskService.Enqueue(firstBackgroundTask);

        var secondBackgroundTask = new BackgroundTask(Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.Queue.Key,
            "secondTask",
            async () =>
            {
                for (var i = 0; i < 3; i++)
                {
                    await Task.Delay(33);
                }
            });

        syncBackgroundTaskService.Enqueue(secondBackgroundTask);

        Assert.NotNull(firstBackgroundTask.WorkProgress);
        Assert.NotNull(secondBackgroundTask.WorkProgress);

        Assert.True(firstBackgroundTask.WorkProgress!.IsCompleted);
        Assert.True(secondBackgroundTask.WorkProgress!.IsCompleted);
    }
}
