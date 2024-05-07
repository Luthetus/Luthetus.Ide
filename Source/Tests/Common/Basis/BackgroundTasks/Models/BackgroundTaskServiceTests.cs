using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Luthetus.Common.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="BackgroundTaskService"/>
/// </summary>
public class BackgroundTaskServiceTests
{
    /// <summary>
    /// <see cref="BackgroundTaskService.EnqueueAsync(IBackgroundTask)"/>
    /// <br/>----<br/>
    /// <see cref="BackgroundTaskService.EnqueueAsync(Key{BackgroundTask}, Key{BackgroundTaskQueue}, string, Func{Task})"/>
    /// <see cref="BackgroundTaskService.RegisterQueue(BackgroundTaskQueue)"/>
    /// <see cref="BackgroundTaskService.DequeueAsync(Key{BackgroundTaskQueue}, CancellationToken)"/>
    /// <see cref="BackgroundTaskService.SetExecutingBackgroundTask(Key{BackgroundTaskQueue}, IBackgroundTask?)"/>
    /// </summary>
    [Fact]
    public async void EnqueueA()
    {
        InitializeBackgroundTaskServiceTests(
            out var backgroundTaskService,
            out var worker,
            out var queue,
            out var cancellationTokenSource,
            out _,
            out _,
            out _);

        Assert.Null(queue.ExecutingBackgroundTask);

        var number = 0;
        Assert.Equal(0, number);

        // foreach backgroundTask (number += 2); from the event.
        // Set executing to the task is +1, then set the executing to null is another +1
        void OnExecutingBackgroundTaskChanged()
        {
            number++;
        }

        queue.ExecutingBackgroundTaskChanged += OnExecutingBackgroundTaskChanged;

        // 1st backgroundTask
        {
            var firstBackgroundTaskKey = Key<BackgroundTask>.NewKey();

            var firstBackgroundTask = new BackgroundTask(
                firstBackgroundTaskKey,
                queue.Key,
                "Abc",
                async () =>
                {
                    // Have both backgroundTasks enqueue'd by invoking 'await Task.Yield();'
                    await Task.Yield();

                    Assert.NotNull(queue.ExecutingBackgroundTask);
                    Assert.Equal(firstBackgroundTaskKey, queue.ExecutingBackgroundTask!.BackgroundTaskKey);

                    // number += 1; from the task.
                    number++;
                });

            backgroundTaskService.EnqueueAsync(firstBackgroundTask);
        }

        // 2nd backgroundTask
        {
            var secondBackgroundTaskKey = Key<BackgroundTask>.NewKey();
            
            backgroundTaskService.EnqueueAsync(
                secondBackgroundTaskKey,
                queue.Key,
                "Zyx",
                () =>
                {
                    // Assert that the first backgroundTask is finished.
                    // (4 happens to be the result given the current unorganized state-mutations that I wrote | (2023-11-19))
                    Assert.Equal(4, number);

                    Assert.NotNull(queue.ExecutingBackgroundTask);
                    Assert.Equal(secondBackgroundTaskKey, queue.ExecutingBackgroundTask!.BackgroundTaskKey);

                    // number += 1; from the task.
                    number++;

                    return Task.CompletedTask;
                });
        }

        await worker.StopAsync(CancellationToken.None);

        queue.ExecutingBackgroundTaskChanged -= OnExecutingBackgroundTaskChanged;
        Assert.Null(queue.ExecutingBackgroundTask);
        Assert.Equal(6, number);
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
            .AddScoped<ILoggerFactory, NullLoggerFactory>()
            .AddScoped<IBackgroundTaskService>(_ => new BackgroundTaskService())
            .AddScoped(sp => new ContinuousBackgroundTaskWorker(
                sp.GetRequiredService<IBackgroundTaskService>(),
                sp.GetRequiredService<ILoggerFactory>()))
            .AddScoped(sp => new BlockingBackgroundTaskWorker(
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

        continuousQueue = new BackgroundTaskQueue(
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(continuousQueue);

        var temporaryBlockingWorker =
            blockingWorker =
            serviceProvider.GetRequiredService<BlockingBackgroundTaskWorker>();

        blockingQueue = new BackgroundTaskQueue(
            BlockingBackgroundTaskWorker.GetQueueKey(),
            BlockingBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(blockingQueue);

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
