using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTask
{
    public Key<IBackgroundTask> BackgroundTaskKey { get; }
    public Key<IBackgroundTaskQueue> QueueKey { get; }
    public string Name { get; }

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
	public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent);
	
	/// <summary>
    /// This method is the actual work item that gets awaited in order to handle the event.
    /// </summary>
	public Task HandleEvent(CancellationToken cancellationToken);
}