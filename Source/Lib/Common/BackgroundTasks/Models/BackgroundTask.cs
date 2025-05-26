using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public struct BackgroundTask : IBackgroundTaskGroup
{
    private readonly Func<ValueTask> _runFunc;

    public BackgroundTask(
        Key<IBackgroundTaskGroup> backgroundTaskKey,
        Func<ValueTask> runFunc)
    {
        _runFunc = runFunc;

        BackgroundTaskKey = backgroundTaskKey;
    }

    /// <summary>
    /// A BackgroundTaskKey of 'Key<IBackgroundTask>.Empty' can be used, but:
    /// - This disables any tracking.
    /// - this disables _taskCompletionSourceMap
    /// </summary>
    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public bool __TaskCompletionSourceWasCreated { get; set; }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        return _runFunc.Invoke();
    }
}