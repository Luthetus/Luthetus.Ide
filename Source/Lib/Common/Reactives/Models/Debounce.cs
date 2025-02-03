namespace Luthetus.Common.RazorLib.Reactives.Models;

/// <summary>
/// The "starter code" for this was copy and pasted from <see cref="ThrottleOptimized"/>.
///
/// This type is meant to fire when the requests cease for some duration of time.
/// i.e.: if one continually fires a request, the <see cref="_workItem"/> will never run.
///
/// Additionally: a second requirement exists where the previously started workItem must be IsCompleted
///               before another workItem can be started.
/// i.e.: if the requests cease for some duration of time, then the previously workItem
///       is awaited. Only then will a new workItem start.
/// </summary> 
public class Debounce<TArgs>
{
    private readonly object _lockWorkItemArgs = new();
	private readonly Func<TArgs, CancellationToken, Task> _workItem;

	/// <summary>
	/// The workItem is invoked with the cancellationToken that is given to this constructor.
	/// As well, the cancellationToken is used to stop the debounce checking that is done in a while loop.
	/// </summary>
    public Debounce(
    	TimeSpan debounceTimeSpan,
    	CancellationToken cancellationToken,
    	Func<TArgs, CancellationToken, Task> workItem)
    {
        DebounceTimeSpan = debounceTimeSpan;
        CancellationToken = cancellationToken;
        _workItem = workItem;
    }
    
    private TArgs _args;
    private DateTime _runDateTime;
    
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

    public TimeSpan DebounceTimeSpan { get; }
    public CancellationToken CancellationToken { get; }
	
	public void Run(TArgs args)
    {
		lock (_lockWorkItemArgs)
		{
			_args = args;
			_runDateTime = DateTime.UtcNow;
            if (_intentToRun)
                return;
                
            _intentToRun = true;
		}
		
        var previousTask = _workItemTask;

        _workItemTask = Task.Run(async () =>
        {
        	// Await the previous work item task.
            await previousTask.ConfigureAwait(false);
        
        	TArgs argsCapture = default;
        	DateTime runDateTimeCapture;
        	
        	while (!CancellationToken.IsCancellationRequested)
        	{
        		await Task.Delay(DebounceTimeSpan);
        		
        		lock (_lockWorkItemArgs)
        		{
        			argsCapture = _args;
					runDateTimeCapture = _runDateTime;
		            _intentToRun = false;
        		}
        		
        		if (DateTime.UtcNow - runDateTimeCapture > DebounceTimeSpan)
        			break;
        	}
        	
        	if (CancellationToken.IsCancellationRequested)
        		return;

			await _workItem.Invoke(argsCapture, CancellationToken)
				.ConfigureAwait(false);
        });
    }
}