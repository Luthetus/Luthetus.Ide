using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskWorker : BackgroundService
{
    private readonly ILogger _logger;
    private bool _hasActiveExecutionActive;

    public BackgroundTaskWorker(
        Key<IBackgroundTaskQueue> queueKey,
        IBackgroundTaskService backgroundTaskService,
        ILoggerFactory loggerFactory)
    {
        QueueKey = queueKey;
        BackgroundTaskService = backgroundTaskService;
        _logger = loggerFactory.CreateLogger<BackgroundTaskWorker>();
    }

    public Key<IBackgroundTaskQueue> QueueKey { get; }
    public IBackgroundTaskService BackgroundTaskService { get; }

    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var currentThread = Thread.CurrentThread;

        _logger.LogInformation("Queued Hosted Service is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var backgroundTask = await BackgroundTaskService
                .DequeueAsync(QueueKey, cancellationToken)
                .ConfigureAwait(false);

            if (backgroundTask is not null)
            {
                try
                {
                    _hasActiveExecutionActive = true;

                    BackgroundTaskService.SetExecutingBackgroundTask(QueueKey, backgroundTask);

					// TODO: Should Task.WhenAll be used here so the delay runs concurrently...
					// ...with the 'HandleEvent'?
					//
					// TODO: Could it be that the reason for ThrottleController locking the UI thread...
					// ...was because I was using Task.WhenAll, and once the tasks actually got awaited,
					// they both finished synchronously somehow, therefore an await never occurred?
					await Task.WhenAll(
							backgroundTask.HandleEvent(cancellationToken),
							Task.Delay(backgroundTask.ThrottleTimeSpan))
						.ConfigureAwait(false);
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
                    
                    _hasActiveExecutionActive = false;
                }
            }
        }

        _logger.LogInformation("Queued Hosted Service is stopping.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await BackgroundTaskService.StopAsync(CancellationToken.None).ConfigureAwait(false);

        // TODO: Polling solution for now, perhaps change to a more optimal solution? (2023-11-19)
        while (BackgroundTaskService.Queues.Any(x => x.ExecutingBackgroundTask is not null) ||
                _hasActiveExecutionActive ||
                // TODO: Here a check is done for if there are background tasks pending for a hacky-concurrency solution
                BackgroundTaskService.Queues.SelectMany(x => x.BackgroundTaskList).Any())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
        }
    }
}