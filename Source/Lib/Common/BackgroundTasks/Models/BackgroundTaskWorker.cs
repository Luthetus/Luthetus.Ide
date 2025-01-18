using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskWorker : BackgroundService
{
    private readonly ILogger _logger;
    private bool _hasActiveExecutionActive;

    public BackgroundTaskWorker(
        Key<IBackgroundTaskQueue> queueKey,
        IBackgroundTaskService backgroundTaskService,
        ILoggerFactory loggerFactory,
        LuthetusHostingKind luthetusHostingKind)
    {
        QueueKey = queueKey;
        BackgroundTaskService = backgroundTaskService;
        _logger = loggerFactory.CreateLogger<BackgroundTaskWorker>();
        LuthetusHostingKind = luthetusHostingKind;
    }

    public Key<IBackgroundTaskQueue> QueueKey { get; }
    public IBackgroundTaskService BackgroundTaskService { get; }
    public Task? StartAsyncTask { get; internal set; }
    public LuthetusHostingKind LuthetusHostingKind { get; }

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
                    
                    await backgroundTask.HandleEvent(cancellationToken).ConfigureAwait(false);
                    await Task.Yield();
                    
                    /*
                    (2025-01-17)
                    ============
                    
                    I think it is a joint effort between the "producer" and "consumer".
                    
                    Blazor WASM perhaps can have the UI freeze due to SynchronizationContext issues.
                    
                    But at the moment, I think the remaining issues
                    are regarding the UI events "yielding" / 'await Task.Yield();'.
                    
                    Because if I hold down the 'j' key,
                    it seems that the WASM UI will just continue 'producing' more 'j' events.
                    
                    But never actually 'consume' them.
                    
                    So I suppose the question is if these 'j' events are ever being dequeued,
                    or if the UI code is running nonstop due to a high load of something?
                    
                    Maybe it is a SynchronizationContext issue...
                    
                    But even in this class, not having 'await Task.Yield();'
                    will result in WASM just handling the background tasks and never updating
                    the UI (if there is a high enough background task load).
                    
                    The multi-threaded UI experience seems greatly improved
                    from today's changes,
                    
                    which is odd cause I was trying to improve single threaded.
                    
                    will continue tomorrow.
                    
                    I wanna type this though
                    I don't wanna just put 'await Task.Yield();'
                    everywhere just for WASM's sake though right?
                    Is that going to slow down a multi-threaded runtime?
                    
                    Anyways.
                    */
                    
                    //await backgroundTask.HandleEvent(cancellationToken);
                    
                    /*if (LuthetusHostingKind == LuthetusHostingKind.Wasm)
                    {
                    	await Task.Yield();
                    }*/

					/*
					// TODO: Should Task.WhenAll be used here so the delay runs concurrently...
					// ...with the 'HandleEvent'?
					//
					// TODO: Could it be that the reason for ThrottleController locking the UI thread...
					// ...was because I was using Task.WhenAll, and once the tasks actually got awaited,
					// they both finished synchronously somehow, therefore an await never occurred?
					var task = backgroundTask.HandleEvent(cancellationToken);
					
					if (task.IsCompleted)
					{
						// If task completes synchronously then yield
						// for any single threaded runtimes so the UI doesn't freeze.
						await Task.Yield();
					}
					else
					{
						await task.ConfigureAwait(false);
					}
					*/
					
					
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
                    BackgroundTaskService.SetExecutingBackgroundTask(QueueKey, null);
                    
                    _hasActiveExecutionActive = false;
                }
            }
        }

        _logger.LogInformation("Queued Hosted Service is stopping.");
    }

	/// <summary>
	/// This method is being created so a unit test can enqueue a background task,
	/// and then await its completion.
	///
	/// At the moment there is 'StopAsync(...)' but this does not suffice as
	/// it is intended to "permanently" stop the <see cref="IBackgroundTaskService"/>
	///
	/// The usage of this in the application code is not advised because its implementation
	/// is simple, naive, and polling.
	///
	/// Given the proper timing of things, one could infinitely enqueue, such that everytime this method
	/// checks if there is a count of 0 background tasks in the <see cref="IBackgroundTaskQueue"/>,
	/// that there could be a newly enqueue'd task therefore this method won't finish.
	/// </summary>
	public async Task FlushAsync(CancellationToken cancellationToken)
	{
		var queue = BackgroundTaskService.GetQueue(QueueKey);

		while (queue.ExecutingBackgroundTask is not null ||
                _hasActiveExecutionActive ||
                queue.Count > 0)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
        }
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