using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTask : IBackgroundTask
{
    private readonly object _syncRoot = new();
    private readonly Func<Task> _runFunc;

    public BackgroundTask(
        Key<BackgroundTask> backgroundTaskKey,
        Key<BackgroundTaskQueue> queueKey,
        string name,
        Func<Task> runFunc)
    {
        _runFunc = runFunc;

        BackgroundTaskKey = backgroundTaskKey;
        QueueKey = queueKey;
        Name = name;
    }

	/// <summary>
	/// Goal: Change BackgroundTask to act similarly to IThrottleEvent #Step 100 (2024-03-11)
	/// -------------------------------------------------------------------------------------
	/// The type <see cref="Luthetus.Common.RazorLib.Reactives.Models.IThrottleEvent"/>
	/// was added a few days earlier than this comment.
	/// After seeing the result of the previously mentioned type and
	/// <see cref="Luthetus.Common.RazorLib.Reactives.Models.ThrottleController"/>.
	/// It seems as though the <see cref="BackgroundTaskService"/> should be re-written
	/// to act in a similar way. That is to say, it should allow for
	/// batching of <see cref="BackgroundTask"/>(s), and allow for a throttle timespan
	/// to be awaited after every <see cref="BackgroundTask"/> finishes. (where this
	/// timespan can be different for every <see cref="BackgroundTask"/>.
	///
	/// Goal: Change BackgroundTask to act similarly to IThrottleEvent #Step 200 (2024-03-11)
	/// -------------------------------------------------------------------------------------
	/// In order to achieve this, I want to ask myself, "what is missing between a BackgroundTask
	/// versus an IThrottleEvent?". I ask this because, it seems these two types share the
	/// same respective purpose.
	///
	/// BackgroundTask misses:
	/// 	- public TimeSpan ThrottleTimeSpan { get; }
	/// 	- public IThrottleEvent? BatchOrDefault(IThrottleEvent oldEvent);
	///
	/// BackgroundTask and IThrottleEvent share:
	/// 	- public Task HandleEvent(CancellationToken cancellationToken);
	/// 		- // The '_runFunc = runFunc;' field from the constructor is this equivalent.
	/// 
	/// IThrottleEvent misses:
	/// 	- public Key<BackgroundTask> BackgroundTaskKey { get; }
	/// 	- public Key<BackgroundTaskQueue> QueueKey { get; }
	///     - public string Name { get; }
	///     - public Task? WorkProgress { get; private set; }
	///
	/// What might a combined type from BackgroundTask, and IThrottleEvent be?
	/// (The name for this type is a bit obnoxious, probably stick with BackgroundTask naming,
	///  and change the type definition)
	///
	/// ThrottleBackgroundTask: 
	/// 	- public Key<BackgroundTask> BackgroundTaskKey { get; }
	/// 	- public Key<BackgroundTaskQueue> QueueKey { get; }
	///     - public string Name { get; }
	/// 	- public TimeSpan ThrottleTimeSpan { get; }
	///     - public Task? WorkProgress { get; private set; }
	/// 	- public Task HandleEvent(CancellationToken cancellationToken);
	/// 	- public IThrottleEvent? BatchOrDefault(IThrottleEvent oldEvent);
	///
	/// In regards to the 'HandleEvent(...)' method, both types contain
	/// something equivalent to one another.
	/// 
	/// BackgroundTask has the 'HandleEvent(...)' method in the form of a 'Func<Task>' named
	/// _runFunc, as a field.
	///
	/// IThrottleEvent has the 'HandleEvent(...)' method itself.
	///
	/// So, should I go with a Func<Task> or an interface method?
	///
	/// The IThrottleEvent 'HandleEvent(...)' method I feel made the code where it was used
	/// a lot cleaner, than if it were a Func<Task>, which was taken as an argument
	/// to the constructor.
	///
	/// The issue with this approach is that one must now make a custom
	/// class implementation of the IThrottleEvent interface everytime they want to
	/// perform a different event.
	///
	/// I think the solution to this would be to use 'HandleEvent(...)' as an
	/// interface method. And then, an implementation of ThrottleBackgroundTask
	/// could accept a Func<Task> in its constructor. Then, invoke the Func<Task>
	/// from 'HandleEvent(...)'.
	///
	/// This allows for 'short-hand' events for small things that don't warrant
	/// a class being made.
	///
	/// While still allowing for complicated event logic to be written
	/// as their own separate class implementation.
	///
	/// Goal: Change BackgroundTask to act similarly to IThrottleEvent #Step 200 (2024-03-11)
	/// -------------------------------------------------------------------------------------
	/// To make these changes to the 'BackgroundTask' type. What needs be done?
	///
	/// The IBackgroundTask needs to be changed to include a 'HandleEvent(...)' method
	/// definition.
	///
	/// Then, this BackgroundTask type needs to implement that method, such that it invoked
	/// the '_runFunc' Func<Task> field.
	///
	/// When it comes to changing the code, could I manage to only change code within the
	/// 'Luthetus.Common.RazorLib.BackgroundTasks.Models' namespace, to start off with?
	///
	/// That is change BackgroundTask without any external code needing to change.
	/// Then after that I can worry about the ThrottleController code.
	/// </summary>
    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; }
    public string Name { get; }
    public Task? WorkProgress { get; private set; }
	public TimeSpan ThrottleTimeSpan { get; } = TimeSpan.Zero;

	public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
	{
		return null;
	}

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        lock (_syncRoot)
        {
            if (WorkProgress is null)
                WorkProgress = _runFunc.Invoke();

            return WorkProgress;
        }
    }
}