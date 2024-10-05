namespace Luthetus.Common.RazorLib.Reactives.Models;

/// <summary>
/// Sometimes when using the <see cref="Throttle"/>,
/// one is just re-using the same Func.
///
/// The current implementation of <see cref="Throttle"/>
/// allows one to 'Run' various Func(s) all through
/// the same throttle.
///
/// And this has its uses.
/// But, if you only want to run 1 Func each 'Run' invocation,
/// why re-allocate a Func object and closure the variables?
///
/// So, this verson will take in the constructor, the Func to 'Run'.
///
/// Then, if any closure is necessary, use the generic argument
/// to this type to define the args.
///
/// Preferably, have the args be a struct, and then
/// a pretty sizable optimization
/// (relative to garbage collection CPU usage and memory)
/// is expected.
///
/// Side note: why does <see cref="Throttle"/> use a Stack
/// for the work items, just to take the top work item, then clear the stack
/// within a lock?
/// |
/// Isn't it equivalent to just set a field from within a lock which is
/// simply a 'Func<CancellationToken, Task>' and avoid the stack entirely?
/// </summary>
public class ThrottleOptimized<TArgs>
{
    private readonly object _lockWorkItemArgs = new();
	private readonly Func<TArgs, CancellationToken, Task> _workItem;

    public ThrottleOptimized(TimeSpan throttleTimeSpan, Func<TArgs, CancellationToken, Task> workItem)
    {
        ThrottleTimeSpan = throttleTimeSpan;
        _workItem = workItem;
    }
    
    private TArgs _args;
    
    /// <summary>
    /// If <see cref="TArgs"/> is a struct,
    /// then a null reference for <see cref="_args"/>
    /// would not suffice to know whether the <see cref="_workItemTask"/>
    /// will be created or not.
    ///
    /// A fix for this could be to move the '_workItemTask' inside the 'lock (_lockWorkItemArgs)'
    /// but it is unknown as to whether this could cause concurrency issues.
    ///
    /// Difference fix could be to mark <see cref="_args"/> as nullable with '?'.
    ///
    /// But it is unknown as to whether this would cause issues relating to
    /// using <see cref="TArgs"/> as a struct in order to optimize memory.
    ///
    /// So, for now (until the previous points are understood fully), this bool is going
    /// to be used to mark whether the '_workItemTask' will be started or not.
    /// In order to permit invocations of <see cref="Run"/> to return early from the lock.
    /// Yet, not have to 'Task.Run' from within the lock.
    /// </summary>
    private bool _intentToRun;

	public Task _workItemTask = Task.CompletedTask;

    public TimeSpan ThrottleTimeSpan { get; }
	
	public void Run(TArgs args)
    {
		lock (_lockWorkItemArgs)
		{
			_args = args;
            if (_intentToRun)
                return;
                
            _intentToRun = true;
		}
		
        var previousTask = _workItemTask;

        _workItemTask = Task.Run(async () =>
        {
            // Await the previous work item task.
            await previousTask.ConfigureAwait(false);

            TArgs argsCapture;
            lock (_lockWorkItemArgs)
            {
                argsCapture = _args;
                _intentToRun = false;
            }

			await Task.WhenAll(
					_workItem.Invoke(argsCapture, CancellationToken.None),
					Task.Delay(ThrottleTimeSpan))
				.ConfigureAwait(false);
        });
    }
}