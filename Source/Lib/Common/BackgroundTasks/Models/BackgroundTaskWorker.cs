using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public sealed class BackgroundTaskWorker
{
    private readonly ILogger _logger;

    public BackgroundTaskWorker(
        BackgroundTaskQueue queue,
        BackgroundTaskService backgroundTaskService,
        ILoggerFactory loggerFactory,
        LuthetusHostingKind luthetusHostingKind)
    {
        Queue = queue;
        BackgroundTaskService = backgroundTaskService;
        _logger = loggerFactory.CreateLogger<BackgroundTaskWorker>();
        LuthetusHostingKind = luthetusHostingKind;
    }
    
    // private IBackgroundTask? _executingBackgroundTask;

    public BackgroundTaskQueue Queue { get; }
    public BackgroundTaskService BackgroundTaskService { get; }
    public Task? StartAsyncTask { get; internal set; }
    public LuthetusHostingKind LuthetusHostingKind { get; }

	/*public IBackgroundTask? ExecutingBackgroundTask
    {
        get => _executingBackgroundTask;
        set
        {
            _executingBackgroundTask = value;
            ExecutingBackgroundTaskChanged?.Invoke();
        }
    }*/
    
    public event Action? ExecutingBackgroundTaskChanged;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(BackgroundTaskWorker)} is starting.");

        while (!cancellationToken.IsCancellationRequested)
        {
        	await Queue.__DequeueSemaphoreSlim.WaitAsync().ConfigureAwait(false);
        	var backgroundTask = Queue.__DequeueOrDefault();

            if (backgroundTask is not null)
            {
                try
                {
                    // ExecutingBackgroundTask = backgroundTask;
                    
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
                	if (backgroundTask.__TaskCompletionSourceWasCreated)
                		BackgroundTaskService.CompleteTaskCompletionSource(backgroundTask.BackgroundTaskKey);
                	
                    // ExecutingBackgroundTask = null;
                }
            }
        }

        _logger.LogInformation("{nameof(BackgroundTaskWorker)} is stopping.");
    }
}