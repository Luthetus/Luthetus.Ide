using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskWorkerTests
{
    [Fact]
    public void Constructor()
    {
        /*
        public BackgroundTaskWorker(
            Key<BackgroundTaskQueue> queueKey, IBackgroundTaskService backgroundTaskService, ILoggerFactory loggerFactory)
         */

        var services = new ServiceCollection()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>();

        var sp = services.BuildServiceProvider();

        var queueKey = ContinuousBackgroundTaskWorker.Queue.Key;

        var backgroundTaskWorker = new ContinuousBackgroundTaskWorker(
            queueKey,
            sp.GetRequiredService<IBackgroundTaskService>(),
            sp.GetRequiredService<ILoggerFactory>());

        // [Fact]
        // public void QueueKey()
        Assert.Equal(queueKey, backgroundTaskWorker.QueueKey);

        // [Fact]
        // public void BackgroundTaskService()
        Assert.NotNull(backgroundTaskWorker.BackgroundTaskService);

        var startCancellationTokenSource = new CancellationTokenSource();
        _ = Task.Run(async () =>  await backgroundTaskWorker.StartAsync(startCancellationTokenSource.Token));
        
        var stopCancellationTokenSource = new CancellationTokenSource();
        _ = Task.Run(async () => await backgroundTaskWorker.StopAsync(stopCancellationTokenSource.Token));

        throw new NotImplementedException("TODO: Testing");
    }
}