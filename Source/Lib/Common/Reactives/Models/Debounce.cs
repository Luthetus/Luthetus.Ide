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

/*

jjjjjjj

j -> Task.Delay(DebounceTimeSpan)

j 

CancellationToken share between
	'j -> Task.Delay(DebounceTimeSpan)'
	'taskStartWorkItem'
	
j ->
	Task.Delay(DebounceTimeSpan, cancellationToken) ->
		Task.Run();

no

j ->
	Task.Run(Task.Delay(DebounceTimeSpan, cancellationToken)); ->
	// if Task.Run is given the cancellationToken it will throw an exception upon cancellation
	// the user probably doesn't want this information so I won't put it on the Task.Run itself.
	// Otherwise every invocation would require me to start a task.
	//
	// Speaking of which, I need to re-use the "intent to start" task rather than stopping
	// it and starting a new task.
	//
	// I presume this would be a massive improvement in all regards
	// considering how often the debounce might fire.
	
j ->
	Step Cancel:
		CancellationTokenSource.Cancel();
	if (intentToRun.IsCompleted)
	{
		// Begin a new intentToRun
		var intentToRun = Task.Run(await () =>
		{
			Task.Delay(DebounceTimeSpan, cancellationToken)
		});
	}
	else
	{
	}
	Step Reuse:
	
No

TimerSpan?

Is there API to re-use the same 'CancellationTokenSource' after cancelling?

Every second, check the timespan of the most recent event.
If mostRecentEvent - CurrentTimeSpan > Delay => start work item

j ->
	if (intentToRun.IsCompleted)
	{
		lock (eventLock)
		{
			_eventTimeSpan = DateTime.UtcNow;
		}
		
		// Begin a new intentToRun
		IntentToRun = Task.Run(async () =>
		{
			// Constructor takes 'CancellationToken' so the outside and tell it to stop
			while (!CancellationToken.IsCancellationRequested)
			{
				await Task.Delay(DebounceTimeSpan);
				
				if (_eventTimeSpan - DateTime.UtcNow > DebounceTimeSpan)
					break;
			}
			
			if (CancellationToken.IsCancellationRequested)
				return;
				
			// Await the previous workItem
			await WorkItem;
			
			WorkItem = workItemFunc.Invoke();
			await WorkItem;
		});
	}
	else
	{
	}
	Step Reuse:

*/

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