using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskWorker : BackgroundService
{
    private readonly ILogger _logger;

    /// <summary>
    /// Don't allow <see cref="StopAsync(CancellationToken)" to execute until
    /// <see cref="StartAsync(CancellationToken)"/>/> has ran.
    /// <br/><br/>
    /// Adding this logic on (2023-10-06) because I want to make an asynchronous
    /// unit test. Perhaps The idea for the unit test itself is a bad decision however.
    /// </summary>
    private readonly SemaphoreSlim _startAsyncHasRanSemaphoreSlim = new(0, 1);

    public BackgroundTaskWorker(
        Key<BackgroundTaskQueue> queueKey,
        IBackgroundTaskService backgroundTaskService,
        ILoggerFactory loggerFactory)
    {
        QueueKey = queueKey;
        BackgroundTaskService = backgroundTaskService;
        _logger = loggerFactory.CreateLogger<BackgroundTaskWorker>();
    }

    private bool _performingGracefulStop;
    private CancellationTokenSource? _stoppingCts;

    public Key<BackgroundTaskQueue> QueueKey { get; }
    public IBackgroundTaskService BackgroundTaskService { get; }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await base.StartAsync(cancellationToken);

        _startAsyncHasRanSemaphoreSlim.Release();
    }

    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Queued Hosted Service is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var backgroundTask = await BackgroundTaskService.DequeueAsync(QueueKey, cancellationToken);

            if (backgroundTask is not null)
            {
                try
                {
                    BackgroundTaskService.SetExecutingBackgroundTask(QueueKey, backgroundTask);
                    await backgroundTask.InvokeWorkItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    var message = ex is OperationCanceledException
                        ? "Task was cancelled {0}." // {0} => WorkItemName
                        : "Error occurred executing {0}."; // {0} => WorkItemName

                    _logger.LogError(ex, message, backgroundTask.Name);
                }
                finally
                {
                    BackgroundTaskService.SetExecutingBackgroundTask(QueueKey, null);
                }
            }

            if (_performingGracefulStop && 0 == BackgroundTaskService.GetQueueCount(QueueKey))
                return;
        }

        _logger.LogInformation("Queued Hosted Service is stopping.");
    }

    public override async Task StopAsync(CancellationToken stopToken)
    {
        // Override StopAsync to wait until the queue is empty, or stopToken is activated.

        await _startAsyncHasRanSemaphoreSlim.WaitAsync();

        _performingGracefulStop = true;

        // After setting '_performingGracefulStop = true;' one might deadlock if the ExecuteTask
        // awaits for 'dequeue', yet there were no queue'd background tasks.
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
            QueueKey, "_performingGracefulStop=true", () => Task.CompletedTask);

        await Task.WhenAny(ExecuteTask, Task.Delay(Timeout.Infinite, stopToken)).ConfigureAwait(false);
        _stoppingCts?.Cancel();
    }
}