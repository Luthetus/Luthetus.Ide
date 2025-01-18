using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskService : IBackgroundTaskService
{
	private readonly Dictionary<Key<IBackgroundTaskQueue>, BackgroundTaskQueue> _queueContainerMap = new();
	
	/// <summary>
	/// Add async blocking enqueue (2024-08-06)
	/// =======================================
	/// The thought here is that the async blocking enqueue could
	/// be done generally two different ways.
	///
	/// Way 1: If async enqueue then store in a Dictionary<Key<IBackgroundTask>, TaskCompletionSource>
	///        Once the 'HandleEvent' is completed then check if there is an entry in the dictionary.
	///        If there is, then complete it.
	///
	/// Way 2: Every BackgroundTask gets a 'TaskCompletionSource?' property which is nullable.
	///        Once the 'HandleEvent' is completed, then check if the 'TaskCompletionSource?' property
	///        is non-null.
	///        If it is non-null, then complete it.
	/// </summary>
    private readonly Dictionary<Key<IBackgroundTask>, TaskCompletionSource> _taskCompletionSourceMap = new();
    
    private readonly object _taskCompletionSourceLock = new();

    private bool _enqueuesAreDisabled;

    public ImmutableArray<IBackgroundTaskQueue> Queues => _queueContainerMap.Values.Select(x => (IBackgroundTaskQueue)x).ToImmutableArray();

    public void Enqueue(IBackgroundTask backgroundTask)
    {
        // TODO: Could there be concurrency issues regarding '_enqueuesAreDisabled'? (2023-11-19)
        if (_enqueuesAreDisabled)
            return;

        _queueContainerMap[backgroundTask.QueueKey]
			.Enqueue(backgroundTask);
    }

    public void Enqueue(Key<IBackgroundTask> taskKey, Key<IBackgroundTaskQueue> queueKey, string name, Func<Task> runFunc)
    {
        Enqueue(new BackgroundTask(taskKey, queueKey, name, runFunc));
    }
    
    public Task EnqueueAsync(IBackgroundTask backgroundTask)
    {
    	if (backgroundTask.BackgroundTaskKey == Key<IBackgroundTask>.Empty)
    	{
    		// TODO: This exception message should be written better.
    		throw new LuthetusCommonException(
    			$"{nameof(EnqueueAsync)} cannot be invoked with an {nameof(IBackgroundTask)} that has an 'backgroundTask.BackgroundTaskKey == Key<IBackgroundTask>.Empty' {nameof(_taskCompletionSourceMap)}");
    	}
    
        // TODO: Could there be concurrency issues regarding '_enqueuesAreDisabled'? (2023-11-19)
        if (_enqueuesAreDisabled)
            return Task.CompletedTask;
            
        TaskCompletionSource taskCompletionSource = new();
            
		lock (_taskCompletionSourceLock)
		{
			if (_taskCompletionSourceMap.ContainsKey(backgroundTask.BackgroundTaskKey))
	        {
	        	var existingTaskCompletionSource = _taskCompletionSourceMap[backgroundTask.BackgroundTaskKey];
	        	
	        	if (!existingTaskCompletionSource.Task.IsCompleted)
	        	{
	        		existingTaskCompletionSource.SetException(new InvalidOperationException("SIMULATED EXCEPTION"));
	        	}
	        	
	        	// Retrospective: Shouldn't this be in an 'else'?
	        	//
	        	// The re-use of the key is not an issue, so long as the previous usage has completed
        		_taskCompletionSourceMap[backgroundTask.BackgroundTaskKey] = taskCompletionSource;
	        }
	        else
	        {
	        	_taskCompletionSourceMap.Add(backgroundTask.BackgroundTaskKey, taskCompletionSource);
	        }
		}

        _queueContainerMap[backgroundTask.QueueKey]
			.Enqueue(backgroundTask);
			
		return taskCompletionSource.Task;
    }

    public Task EnqueueAsync(Key<IBackgroundTask> taskKey, Key<IBackgroundTaskQueue> queueKey, string name, Func<Task> runFunc)
    {
        return EnqueueAsync(new BackgroundTask(taskKey, queueKey, name, runFunc));
    }
    
    public void CompleteTaskCompletionSource(Key<IBackgroundTask> backgroundTaskKey)
    {
    	lock (_taskCompletionSourceLock)
		{
			if (_taskCompletionSourceMap.ContainsKey(backgroundTaskKey))
	        {
	        	var existingTaskCompletionSource = _taskCompletionSourceMap[backgroundTaskKey];
	        	
	        	if (!existingTaskCompletionSource.Task.IsCompleted)
	        	{
	        		existingTaskCompletionSource.SetResult();
	        	}
	        	
	        	_taskCompletionSourceMap.Remove(backgroundTaskKey);
	        }
		}
    }

	public IBackgroundTask? Dequeue(Key<IBackgroundTaskQueue> queueKey)
    {
        var queue = _queueContainerMap[queueKey];
        return queue.DequeueOrDefault();
    }

    public async Task<IBackgroundTask?> DequeueAsync(
        Key<IBackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken)
    {
        var queue = _queueContainerMap[queueKey];
		await queue.DequeueSemaphoreSlim.WaitAsync();//.ConfigureAwait(false);
        return queue.DequeueOrDefault();
    }

    public void RegisterQueue(IBackgroundTaskQueue queue)
    {
        _queueContainerMap.Add(queue.Key, (BackgroundTaskQueue)queue);
    }

    public void SetExecutingBackgroundTask(
        Key<IBackgroundTaskQueue> queueKey,
        IBackgroundTask? backgroundTask)
    {
        var queue = _queueContainerMap[queueKey];

        queue.ExecutingBackgroundTask = backgroundTask;
    }
    
    public IBackgroundTaskQueue GetQueue(Key<IBackgroundTaskQueue> queueKey)
    {
        return _queueContainerMap[queueKey];
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _enqueuesAreDisabled = true;

        // TODO: Polling solution for now, perhaps change to a more optimal solution? (2023-11-19)
        while (_queueContainerMap.Values.SelectMany(x => x.BackgroundTaskList).Any())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
        }
    }
}
