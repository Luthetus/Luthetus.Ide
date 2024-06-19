using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BackgroundTask : IBackgroundTask
{
    private readonly object _syncRoot = new();
    private readonly Func<Task> _runFunc;

    public BackgroundTask(
        Key<IBackgroundTask> backgroundTaskKey,
        Key<IBackgroundTaskQueue> queueKey,
        string name,
        Func<Task> runFunc)
    {
        _runFunc = runFunc;

        BackgroundTaskKey = backgroundTaskKey;
        QueueKey = queueKey;
        Name = name;
    }

    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; }
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