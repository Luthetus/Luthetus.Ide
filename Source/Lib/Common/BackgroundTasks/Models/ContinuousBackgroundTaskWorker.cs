using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public sealed class ContinuousBackgroundTaskWorker
{
    private readonly ILogger _logger;

    public ContinuousBackgroundTaskWorker(
        BackgroundTaskQueue queue,
        BackgroundTaskService backgroundTaskService,
        ILoggerFactory loggerFactory,
        LuthetusHostingKind luthetusHostingKind)
    {
        Queue = queue;
        BackgroundTaskService = backgroundTaskService;
        _logger = loggerFactory.CreateLogger<ContinuousBackgroundTaskWorker>();
        LuthetusHostingKind = luthetusHostingKind;
    }

    public BackgroundTaskQueue Queue { get; }
    public BackgroundTaskService BackgroundTaskService { get; }
    public Task? StartAsyncTask { get; internal set; }
    public LuthetusHostingKind LuthetusHostingKind { get; }
    
    public event Action? ExecutingBackgroundTaskChanged;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(ContinuousBackgroundTaskWorker)} is starting.");

		while (!cancellationToken.IsCancellationRequested)
		{
			try
            {
		        while (!cancellationToken.IsCancellationRequested)
		        {
		        	await Queue.__DequeueSemaphoreSlim.WaitAsync().ConfigureAwait(false);
	                await Queue.__DequeueOrDefault().HandleEvent().ConfigureAwait(false);
	                await Task.Yield();
		        }
	        }
	        catch (Exception ex)
            {
                var message = ex is OperationCanceledException
                    ? "Task was cancelled {0}." // {0} => WorkItemName
                    : "Error occurred executing {0}."; // {0} => WorkItemName

                _logger.LogError(ex, message, "(backgroundTask.Name was here)");
				Console.WriteLine($"ERROR on (backgroundTask.Name was here): {ex.ToString()}");
            }
        }

        _logger.LogInformation("{nameof(BackgroundTaskWorker)} is stopping.");
    }
}