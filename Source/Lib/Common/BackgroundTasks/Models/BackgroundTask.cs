using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTask : IBackgroundTask
{
    private readonly Func<ValueTask> _runFunc;

    public BackgroundTask(
        Key<IBackgroundTask> backgroundTaskKey,
        Key<IBackgroundTaskQueue> queueKey,
        string name,
        Func<ValueTask> runFunc)
    {
        _runFunc = runFunc;

        BackgroundTaskKey = backgroundTaskKey;
        QueueKey = queueKey;
        Name = name;
    }

    /// <summary>
    /// A BackgroundTaskKey of 'Key<IBackgroundTask>.Empty' can be used, but:
    /// - This disables any tracking.
    /// - this disables _taskCompletionSourceMap
    /// </summary>
    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; }
    public string Name { get; }
    public bool EarlyBatchEnabled { get; set; }
    public bool LateBatchEnabled { get; set; }

	public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
	{
		return null;
	}
	
	public IBackgroundTask? LateBatchOrDefault(IBackgroundTask oldEvent)
	{
		return null;
	}

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        return _runFunc.Invoke();
    }
}