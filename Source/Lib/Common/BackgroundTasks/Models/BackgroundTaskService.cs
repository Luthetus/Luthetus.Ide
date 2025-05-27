using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public sealed class BackgroundTaskService
{
    private readonly Dictionary<Key<IBackgroundTaskGroup>, TaskCompletionSource> _taskCompletionSourceMap = new();
    private readonly object _taskCompletionSourceLock = new();
    
	/// <summary>
	/// Generally speaking: Presume that the ContinuousTaskWorker is "always ready" to run the next task that gets enqueued.
	/// </summary>
	public ContinuousBackgroundTaskWorker ContinuousWorker { get; private set; }
	/// <summary>
	/// Generally speaking: Presume that the ContinuousTaskWorker is "always ready" to run the next task that gets enqueued.
	/// </summary>
	public BackgroundTaskQueue ContinuousQueue { get; private set; }
	
	/// <summary>
	/// Generally speaking: Presume that the IndefiniteTaskWorker is NOT ready to run the next task that gets enqueued.
	/// </summary>
    public IndefiniteBackgroundTaskWorker IndefiniteWorker { get; private set; }
    /// <summary>
	/// Generally speaking: Presume that the IndefiniteTaskWorker is NOT ready to run the next task that gets enqueued.
	/// </summary>
    public BackgroundTaskQueue IndefiniteQueue { get; private set; }

	public void Continuous_EnqueueGroup(IBackgroundTaskGroup backgroundTaskGroup)
	{
		ContinuousQueue.Enqueue(backgroundTaskGroup);
	}
	
	public void Indefinite_EnqueueGroup(IBackgroundTaskGroup backgroundTaskGroup)
	{
		IndefiniteQueue.Enqueue(backgroundTaskGroup);
	}

    public Task Indefinite_EnqueueAsync(IBackgroundTaskGroup backgroundTask)
    {
    	backgroundTask.__TaskCompletionSourceWasCreated = true;
    	
    	if (backgroundTask.BackgroundTaskKey == Key<IBackgroundTaskGroup>.Empty)
    	{
    		throw new LuthetusCommonException(
    			$"{nameof(Indefinite_EnqueueAsync)} cannot be invoked with an {nameof(IBackgroundTaskGroup)} that has a 'BackgroundTaskKey == Key<IBackgroundTask>.Empty'. An empty key disables tracking, and task completion source. The non-async Enqueue(...) will still work however.");
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

        IndefiniteQueue.Enqueue(backgroundTask);
			
		return taskCompletionSource.Task;
    }

    public Task Indefinite_EnqueueAsync(Key<IBackgroundTaskGroup> taskKey, Key<BackgroundTaskQueue> queueKey, string name, Func<ValueTask> runFunc)
    {
        return Indefinite_EnqueueAsync(new BackgroundTask(taskKey, runFunc));
    }
    
    public void CompleteTaskCompletionSource(Key<IBackgroundTaskGroup> backgroundTaskKey)
    {
    	lock (_taskCompletionSourceLock)
		{
			if (_taskCompletionSourceMap.ContainsKey(backgroundTaskKey))
	        {
	        	var existingTaskCompletionSource = _taskCompletionSourceMap[backgroundTaskKey];
	        	
	        	if (!existingTaskCompletionSource.Task.IsCompleted)
	        		existingTaskCompletionSource.SetResult();
	        	
	        	_taskCompletionSourceMap.Remove(backgroundTaskKey);
	        }
		}
    }

    public void SetContinuousWorker(ContinuousBackgroundTaskWorker worker)
    {
    	ContinuousWorker = worker;
    }
    
    public void SetContinuousQueue(BackgroundTaskQueue queue)
    {
    	ContinuousQueue = queue;
    }
    
    public void SetIndefiniteWorker(IndefiniteBackgroundTaskWorker worker)
    {
    	IndefiniteWorker = worker;
    }
    
    public void SetIndefiniteQueue(BackgroundTaskQueue queue)
    {
    	IndefiniteQueue = queue;
    }
}
