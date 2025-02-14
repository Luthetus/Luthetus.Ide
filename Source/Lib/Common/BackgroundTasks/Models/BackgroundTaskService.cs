using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTaskService : IBackgroundTaskService
{
	private readonly Dictionary<Key<IBackgroundTaskQueue>, BackgroundTaskQueue> _queueContainerMap = new();
    private readonly Dictionary<Key<IBackgroundTask>, TaskCompletionSource> _taskCompletionSourceMap = new();
    
    private readonly object _taskCompletionSourceLock = new();
    
	/// <summary>
	/// Generally speaking: Presume that the ContinuousTaskWorker is "always ready" to run the next task that gets enqueued.
	/// </summary>
	public BackgroundTaskWorker ContinuousTaskWorker { get; private set; }
	/// <summary>
	/// Generally speaking: Presume that the IndefiniteTaskWorker is NOT ready to run the next task that gets enqueued.
	/// </summary>
    public BackgroundTaskWorker IndefiniteTaskWorker { get; private set; }

	public List<IBackgroundTaskQueue> GetQueues() => _queueContainerMap.Values.Select(x => (IBackgroundTaskQueue)x).ToList();

    public void Enqueue(IBackgroundTask backgroundTask)
    {
        _queueContainerMap[backgroundTask.QueueKey].Enqueue(backgroundTask);
    }

    public void Enqueue(Key<IBackgroundTask> taskKey, Key<IBackgroundTaskQueue> queueKey, string name, Func<ValueTask> runFunc)
    {
        Enqueue(new BackgroundTask(taskKey, queueKey, name, runFunc));
    }
    
    public Task EnqueueAsync(IBackgroundTask backgroundTask)
    {
    	backgroundTask.__TaskCompletionSourceWasCreated = true;
    	
    	if (backgroundTask.BackgroundTaskKey == Key<IBackgroundTask>.Empty)
    	{
    		throw new LuthetusCommonException(
    			$"{nameof(EnqueueAsync)} cannot be invoked with an {nameof(IBackgroundTask)} that has a 'BackgroundTaskKey == Key<IBackgroundTask>.Empty'. An empty key disables tracking, and task completion source. The non-async Enqueue(...) will still work however.");
    	}

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

        _queueContainerMap[backgroundTask.QueueKey].Enqueue(backgroundTask);
			
		return taskCompletionSource.Task;
    }

    public Task EnqueueAsync(Key<IBackgroundTask> taskKey, Key<IBackgroundTaskQueue> queueKey, string name, Func<ValueTask> runFunc)
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
        return queue.__DequeueOrDefault();
    }

    public async Task<IBackgroundTask?> DequeueAsync(
        Key<IBackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken)
    {
        var queue = _queueContainerMap[queueKey];
		await queue.__DequeueSemaphoreSlim.WaitAsync().ConfigureAwait(false);
        return queue.__DequeueOrDefault();
    }

    public void RegisterQueue(IBackgroundTaskQueue queue)
    {
        _queueContainerMap.Add(queue.Key, (BackgroundTaskQueue)queue);
    }

    public IBackgroundTaskQueue GetQueue(Key<IBackgroundTaskQueue> queueKey)
    {
        return _queueContainerMap[queueKey];
    }
    
    public void SetContinuousTaskWorker(BackgroundTaskWorker continuousTaskWorker)
    {
    	ContinuousTaskWorker = continuousTaskWorker;
    }
    
    public void SetIndefiniteTaskWorker(BackgroundTaskWorker indefiniteTaskWorker)
    {
    	IndefiniteTaskWorker = indefiniteTaskWorker;
    }
}
