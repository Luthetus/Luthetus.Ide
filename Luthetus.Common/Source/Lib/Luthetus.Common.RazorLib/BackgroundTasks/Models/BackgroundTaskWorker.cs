using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskWorker : BackgroundService
{
    private readonly ILogger _logger;

    public BackgroundTaskWorker(
        Key<BackgroundTaskQueue> queueKey,
        IBackgroundTaskService backgroundTaskService,
        ILoggerFactory loggerFactory)
    {
        QueueKey = queueKey;
        BackgroundTaskService = backgroundTaskService;
        _logger = loggerFactory.CreateLogger<BackgroundTaskWorker>();
    }

    public Key<BackgroundTaskQueue> QueueKey { get; }
    public IBackgroundTaskService BackgroundTaskService { get; }

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
        }

        _logger.LogInformation("Queued Hosted Service is stopping.");
    }
}