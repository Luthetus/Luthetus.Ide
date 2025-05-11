using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public sealed class BackgroundTaskService
{
	private readonly Dictionary<Key<BackgroundTaskQueue>, BackgroundTaskQueue> _queueContainerMap = new();
    private readonly Dictionary<Key<IBackgroundTaskGroup>, TaskCompletionSource> _taskCompletionSourceMap = new();
    
    private readonly object _taskCompletionSourceLock = new();
    
	/// <summary>
	/// Generally speaking: Presume that the ContinuousTaskWorker is "always ready" to run the next task that gets enqueued.
	/// </summary>
	public BackgroundTaskWorker ContinuousTaskWorker { get; private set; }
	/// <summary>
	/// Generally speaking: Presume that the IndefiniteTaskWorker is NOT ready to run the next task that gets enqueued.
	/// </summary>
    public BackgroundTaskWorker IndefiniteTaskWorker { get; private set; }

	public List<BackgroundTaskQueue> GetQueues() => _queueContainerMap.Values.Select(x => (BackgroundTaskQueue)x).ToList();

	public void EnqueueGroup(IBackgroundTaskGroup backgroundTaskGroup)
	{
		_queueContainerMap[backgroundTaskGroup.QueueKey].Enqueue(backgroundTaskGroup);
	}
    
    public Task EnqueueAsync(IBackgroundTaskGroup backgroundTask)
    {
    	backgroundTask.__TaskCompletionSourceWasCreated = true;
    	
    	if (backgroundTask.BackgroundTaskKey == Key<IBackgroundTaskGroup>.Empty)
    	{
    		throw new LuthetusCommonException(
    			$"{nameof(EnqueueAsync)} cannot be invoked with an {nameof(IBackgroundTaskGroup)} that has a 'BackgroundTaskKey == Key<IBackgroundTask>.Empty'. An empty key disables tracking, and task completion source. The non-async Enqueue(...) will still work however.");
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

    public Task EnqueueAsync(Key<IBackgroundTaskGroup> taskKey, Key<BackgroundTaskQueue> queueKey, string name, Func<ValueTask> runFunc)
    {
        return EnqueueAsync(new BackgroundTask(taskKey, queueKey, name, runFunc));
    }
    
    public void CompleteTaskCompletionSource(Key<IBackgroundTaskGroup> backgroundTaskKey)
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

	public IBackgroundTaskGroup? Dequeue(Key<BackgroundTaskQueue> queueKey)
    {
        var queue = _queueContainerMap[queueKey];
        return queue.__DequeueOrDefault();
    }

    public async Task<IBackgroundTaskGroup?> DequeueAsync(
        Key<BackgroundTaskQueue> queueKey,
        CancellationToken cancellationToken)
    {
        var queue = _queueContainerMap[queueKey];
		await queue.__DequeueSemaphoreSlim.WaitAsync().ConfigureAwait(false);
        return queue.__DequeueOrDefault();
    }

    public void RegisterQueue(BackgroundTaskQueue queue)
    {
        _queueContainerMap.Add(queue.Key, (BackgroundTaskQueue)queue);
    }

    public BackgroundTaskQueue GetQueue(Key<BackgroundTaskQueue> queueKey)
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
