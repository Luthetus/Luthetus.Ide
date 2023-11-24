using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="BackgroundTaskWorker"/>
/// </summary>
public class BackgroundTaskWorkerTests
{
    /// <summary>
    /// <see cref="BackgroundTaskWorker(Key{BackgroundTaskQueue}, IBackgroundTaskService, ILoggerFactory)"/>
    /// <br/>----<br/>
    /// <see cref="BackgroundTaskWorker.QueueKey"/>
    /// <see cref="BackgroundTaskWorker.BackgroundTaskService"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var services = new ServiceCollection()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>()
            .AddSingleton<IBackgroundTaskService, BackgroundTaskServiceSynchronous>();

        var sp = services.BuildServiceProvider();

        var queueKey = ContinuousBackgroundTaskWorker.GetQueueKey();

        var backgroundTaskWorker = new ContinuousBackgroundTaskWorker(
            sp.GetRequiredService<IBackgroundTaskService>(),
            sp.GetRequiredService<ILoggerFactory>());

        Assert.Equal(queueKey, backgroundTaskWorker.QueueKey);
        Assert.NotNull(backgroundTaskWorker.BackgroundTaskService);

        var startCancellationTokenSource = new CancellationTokenSource();
        _ = Task.Run(async () =>  await backgroundTaskWorker.StartAsync(startCancellationTokenSource.Token));
        
        var stopCancellationTokenSource = new CancellationTokenSource();
        _ = Task.Run(async () => await backgroundTaskWorker.StopAsync(stopCancellationTokenSource.Token));
    }
}