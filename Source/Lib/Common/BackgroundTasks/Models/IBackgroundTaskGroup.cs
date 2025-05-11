using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTaskGroup
{
	public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; }
    public Key<BackgroundTaskQueue> QueueKey { get; }
    public string Name { get; }
    public bool EarlyBatchEnabled { get; }
    
    /// <summary>
    /// Optimize 'IBackgroundTaskService.CompleteTaskCompletionSource(Key<IBackgroundTask> taskKey);'
    ///
    /// Which is ran after every dequeued background task finishes.
    /// Instead of having to
    /// - Enter a 'lock (...) {...}
    /// - Check if an entry exists with the BackgroundTaskKey
    ///
    /// Just check if this bool is true,
    /// if it then you should invoke the method and
    /// do the lock and check entry exists etc...
    /// </summary>
    public bool __TaskCompletionSourceWasCreated { get; set; }

    /// <summary>
    /// Before a throttle event is enqueued, this method is invoked.
    /// This allows for batching of 'this' throttle event instance,
    /// and the last entry of the queue.<br/><br/>
    /// 
    /// This method is invoked from within a 'lock(_queue) { ... }'.<br/><br/>
    /// 
    /// Example use: one reads an entry from the queue, its an event that write out a character.
    ///              Perhaps this throttle event also writes out a character.
    ///              Instead of invoking 'text.Insert(character)' two times.
    ///              One might want to invoke 'text.InsertRange(bothCharacters)';<br/><br/>
    ///              
    /// Returning null means enqueue the recentEvent without any batching,
    /// (leave the old event as it was within the queue)
    /// </summary>
	public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent);
	
	/// <summary>
    /// This method is the actual work item that gets awaited in order to handle the event.
    /// </summary>
	public ValueTask HandleEvent(CancellationToken cancellationToken);
}
