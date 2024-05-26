using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskWorker : BackgroundService
{
    private readonly ILogger _logger;
    private bool _hasActiveExecutionActive;

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
		Console.WriteLine("worker-ExecuteAsync");
        _logger.LogInformation("Queued Hosted Service is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var backgroundTask = await BackgroundTaskService
                .DequeueAsync(QueueKey, cancellationToken);

            if (backgroundTask is not null)
            {
                try
                {
                    _hasActiveExecutionActive = true;

                    BackgroundTaskService.SetExecutingBackgroundTask(QueueKey, backgroundTask);

					// (2024-05-25) I think it just clicked to me what '.ConfigureAwait(false)' does.
					// By using '.ConfigureAwait(false)' here, I am defeating the purpose of my BackgroundTaskWorker.
					// |
					// Because, '.ConfigureAwait(false)' is saying, "Anyone can pick back up from where I left off, once
					// this Task is awaited."
					// |
					// It was confusing to me because, '.ConfigureAwait(false)' was not permitting the
					// continued execution of the method, without having awaited. That is some sort of
					// fire and forget related topic.
					// |
					// But in my head, I couldn't determine what it was doing, if not related to that.
					// |
					// Dog wants me
					//
					// I have an exception throwing if the text editor ends up creating
					// two instances of 'IEditContext'.
					//
					// This exception is happening nearly everytime I press a key on my keyboard.

					var handleEventTask = backgroundTask.HandleEvent(cancellationToken);
					var throttleTimeSpanTask = Task.Delay(backgroundTask.ThrottleTimeSpan);
					await Task.WhenAll(handleEventTask, throttleTimeSpanTask);
                }
				catch (LuthetusFatalException ex)
				{
					throw;
				}
                catch (Exception ex)
                {
                    var message = ex is OperationCanceledException
                        ? "Task was cancelled {0}." // {0} => WorkItemName
                        : "Error occurred executing {0}."; // {0} => WorkItemName

					Console.WriteLine(backgroundTask.Name + "_" + ex.ToString());
                    _logger.LogError(ex, message, backgroundTask.Name);
                }
                finally
                {
                    BackgroundTaskService.SetExecutingBackgroundTask(QueueKey, null);
                    
                    _hasActiveExecutionActive = false;
                }
            }
        }

		Console.WriteLine("leaving-worker-ExecuteAsync");
        _logger.LogInformation("Queued Hosted Service is stopping.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
		Console.WriteLine("entering-worker-StopAsync");
        await BackgroundTaskService.StopAsync(CancellationToken.None);

        // TODO: Polling solution for now, perhaps change to a more optimal solution? (2023-11-19)
        while (BackgroundTaskService.Queues.Any(x => x.ExecutingBackgroundTask is not null) ||
                _hasActiveExecutionActive ||
                // TODO: Here a check is done for if there are background tasks pending for a hacky-concurrency solution
                BackgroundTaskService.Queues.SelectMany(x => x.BackgroundTasks).Any())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        }

		Console.WriteLine("leaving-worker-StopAsync");
    }
}