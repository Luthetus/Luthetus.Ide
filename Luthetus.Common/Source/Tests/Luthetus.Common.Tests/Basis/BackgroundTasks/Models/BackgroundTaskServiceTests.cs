using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Luthetus.Common.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="BackgroundTaskService"/>
/// </summary>
public class BackgroundTaskServiceTests
{
    /// <summary>
    /// <see cref="BackgroundTaskService.Enqueue(IBackgroundTask)"/>
    /// <br/>----<br/>
    /// <see cref="BackgroundTaskService.RegisterQueue(BackgroundTaskQueue)"/>
    /// </summary>
    [Fact]
    public void EnqueueA()
    {
        InitializeBackgroundTaskServiceTests(
            out var backgroundTaskService,
            out var worker,
            out var queue,
            out var cancellationTokenSource,
            out _,
            out _,
            out _);

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskService.Enqueue(Key{BackgroundTask}, Key{BackgroundTaskQueue}, string, Func{Task})"/>
    /// </summary>
    [Fact]
    public void EnqueueB()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskService.DequeueAsync(Key{BackgroundTaskQueue}, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void DequeueAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="BackgroundTaskService.SetExecutingBackgroundTask(Key{BackgroundTaskQueue}, IBackgroundTask?)"/>
    /// </summary>
    [Fact]
    public void SetExecutingBackgroundTask()
    {
        throw new NotImplementedException();
    }

    private void InitializeBackgroundTaskServiceTests(
        out BackgroundTaskService backgroundTaskService,
        out BackgroundTaskWorker continuousWorker,
        out BackgroundTaskQueue continuousQueue,
        out CancellationTokenSource continuousCancellationTokenSource,
        out BackgroundTaskWorker blockingWorker,
        out BackgroundTaskQueue blockingQueue,
        out CancellationTokenSource blockingCancellationTokenSource)
    {
        var services = new ServiceCollection()
            .AddScoped<IBackgroundTaskService>(_ => new BackgroundTaskService())
            .AddScoped(sp => new ContinuousBackgroundTaskWorker(
                ContinuousBackgroundTaskWorker.Queue.Key,
                sp.GetRequiredService<IBackgroundTaskService>(),
                sp.GetRequiredService<ILoggerFactory>()))
            .AddScoped(sp => new BlockingBackgroundTaskWorker(
                BlockingBackgroundTaskWorker.Queue.Key,
                sp.GetRequiredService<IBackgroundTaskService>(),
                sp.GetRequiredService<ILoggerFactory>()))
            .AddFluxor(options => options.ScanAssemblies(typeof(IBackgroundTaskService).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        backgroundTaskService = (BackgroundTaskService)serviceProvider.GetRequiredService<IBackgroundTaskService>();

        var temporaryContinuousWorker =
            continuousWorker =
            serviceProvider.GetRequiredService<ContinuousBackgroundTaskWorker>();

        continuousQueue = ContinuousBackgroundTaskWorker.Queue;
        backgroundTaskService.RegisterQueue(ContinuousBackgroundTaskWorker.Queue);

        var temporaryBlockingWorker =
            blockingWorker =
            serviceProvider.GetRequiredService<BlockingBackgroundTaskWorker>();

        blockingQueue = BlockingBackgroundTaskWorker.Queue;
        backgroundTaskService.RegisterQueue(BlockingBackgroundTaskWorker.Queue);

        continuousCancellationTokenSource = new CancellationTokenSource();
        var continuousCancellationToken = continuousCancellationTokenSource.Token;

        _ = Task.Run(async () => await temporaryContinuousWorker
                    .StartAsync(continuousCancellationToken)
                    .ConfigureAwait(false),
                continuousCancellationToken);

        blockingCancellationTokenSource = new CancellationTokenSource();
        var blockingCancellationToken = blockingCancellationTokenSource.Token;

        _ = Task.Run(async () => await temporaryBlockingWorker
                    .StartAsync(blockingCancellationToken)
                    .ConfigureAwait(false),
                blockingCancellationToken);
    }
}
