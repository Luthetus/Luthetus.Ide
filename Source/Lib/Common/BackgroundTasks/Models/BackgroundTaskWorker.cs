using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskWorker : BackgroundService
{
    private readonly ILogger _logger;

    public BackgroundTaskWorker(
        IBackgroundTaskQueue queue,
        IBackgroundTaskService backgroundTaskService,
        ILoggerFactory loggerFactory,
        LuthetusHostingKind luthetusHostingKind)
    {
        Queue = queue;
        BackgroundTaskService = backgroundTaskService;
        _logger = loggerFactory.CreateLogger<BackgroundTaskWorker>();
        LuthetusHostingKind = luthetusHostingKind;
    }
    
    private IBackgroundTask? _executingBackgroundTask;

    public IBackgroundTaskQueue Queue { get; }
    public IBackgroundTaskService BackgroundTaskService { get; }
    public Task? StartAsyncTask { get; internal set; }
    public LuthetusHostingKind LuthetusHostingKind { get; }

	public IBackgroundTask? ExecutingBackgroundTask
    {
        get => _executingBackgroundTask;
        set
        {
            _executingBackgroundTask = value;
            ExecutingBackgroundTaskChanged?.Invoke();
        }
    }
    
    public event Action? ExecutingBackgroundTaskChanged;

    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Queued Hosted Service is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var backgroundTask = await BackgroundTaskService
                .DequeueAsync(Queue.Key, cancellationToken)
                .ConfigureAwait(false);

            if (backgroundTask is not null)
            {
                try
                {
                    ExecutingBackgroundTask = backgroundTask;
                    
                    await backgroundTask.HandleEvent(cancellationToken).ConfigureAwait(false);
                    await Task.Yield();
                }
                catch (Exception ex)
                {
                    var message = ex is OperationCanceledException
                        ? "Task was cancelled {0}." // {0} => WorkItemName
                        : "Error occurred executing {0}."; // {0} => WorkItemName

                    _logger.LogError(ex, message, backgroundTask.Name);
					Console.WriteLine($"ERROR on {backgroundTask.Name}: {ex.ToString()}");
                }
                finally
                {
                	BackgroundTaskService.CompleteTaskCompletionSource(backgroundTask.BackgroundTaskKey);
                    ExecutingBackgroundTask = null;
                }
            }
        }

        _logger.LogInformation("Queued Hosted Service is stopping.");
    }
}