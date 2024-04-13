using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTask
{
    public Key<BackgroundTask> BackgroundTaskKey { get; }
    public Key<BackgroundTaskQueue> QueueKey { get; }
    public string Name { get; }
    public Task? WorkProgress { get; }

    /// <summary>
    /// The <see cref="ThrottleController"/> will await this prior to
    /// reading, and invoking <see cref="HandleEvent(CancellationToken)"/>, on the next event from the queue.<br/><br/>
    /// 
    /// Example use: One might want to throttle an 'onmousemove' event, longer than that of a 'onmousedown' event.
    ///              In particular, one could set the 'onmousemove' to 50 miliseconds, and the 'onmousedown' to 0 miliseconds.
    ///              The reasoning here is that 'onmousemove' happens far more often than 'onmousedown'.<br/><br/>
    ///             |<br/><br/>
    ///              Further details: one likely wants to have all the UI events be handled in the order that they occurred.
    ///              Since all the delays are awaited from the same <see cref="ThrottleController"/>,
    ///              one can set <see cref="ThrottleTimeSpan"/> relative to the event.<br/><br/>
    /// </summary>
	public TimeSpan ThrottleTimeSpan { get; }

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