using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="BackgroundTaskServiceSynchronous"/>
/// </summary>
public class BackgroundTaskServiceSynchronousTests
{
    /// <summary>
    /// <see cref="BackgroundTaskServiceSynchronous.ExecutingBackgroundTask"/>
    /// <br/>----<br/>
    /// <see cref="BackgroundTaskServiceSynchronous.ExecutingBackgroundTaskChanged"/>
    /// <see cref="BackgroundTaskServiceSynchronous.Enqueue(IBackgroundTask)"/>
    /// <see cref="BackgroundTaskServiceSynchronous.SetExecutingBackgroundTask(Key{BackgroundTaskQueue}, IBackgroundTask?)"/>
    /// <see cref="BackgroundTaskServiceSynchronous.RegisterQueue(BackgroundTaskQueue)"/>
    /// <see cref="BackgroundTaskServiceSynchronous.DequeueAsync(Key{BackgroundTaskQueue}, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void ExecutingBackgroundTask()
    {
        InitializeBackgroundTaskServiceSynchronousTests(
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

        // For the 'BackgroundTaskServiceSynchronous', the DequeueAsync method should
        // do nothing. This is because once enqueued the task is immediately invoked.
        backgroundTaskService
            .DequeueAsync(queue.Key, CancellationToken.None)
            .Wait();
    }

    /// <summary>
    /// <see cref="BackgroundTaskServiceSynchronous.Enqueue(Key{BackgroundTask}, Key{BackgroundTaskQueue}, string, Func{Task})"/>
    /// </summary>
    [Fact]
    public void EnqueueB()
    {
        InitializeBackgroundTaskServiceSynchronousTests(
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

        backgroundTaskService.Enqueue(
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

        Assert.Equal(3, number);
        Assert.Null(queue.ExecutingBackgroundTask);

        queue.ExecutingBackgroundTaskChanged -= OnExecutingBackgroundTaskChanged;

        // For the 'BackgroundTaskServiceSynchronous', the DequeueAsync method should
        // do nothing. This is because once enqueued the task is immediately invoked.
        backgroundTaskService
            .DequeueAsync(queue.Key, CancellationToken.None)
            .Wait();
    }

    private void InitializeBackgroundTaskServiceSynchronousTests(
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