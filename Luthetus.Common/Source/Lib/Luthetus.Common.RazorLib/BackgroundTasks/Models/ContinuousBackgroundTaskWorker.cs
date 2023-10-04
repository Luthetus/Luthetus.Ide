using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.Logging;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// <summary>
/// The <see cref="ContinuousBackgroundTaskWorker"/> should "always" be available.
/// That is to say, if one needs to execute a task for an indefinitely long amount of,
/// then one should use <see cref="BlockingBackgroundTaskWorker"/> instead.
/// </summary>
public class ContinuousBackgroundTaskWorker : BackgroundTaskWorker
{
    public static BackgroundTaskQueue Queue { get; } = new BackgroundTaskQueue(
        Key<BackgroundTaskQueue>.NewKey(),
        "ContinuousBackgroundTaskWorker");

    public ContinuousBackgroundTaskWorker(
            Key<BackgroundTaskQueue> queueKey,
            IBackgroundTaskService backgroundTaskService,
            ILoggerFactory loggerFactory)
        : base(queueKey, backgroundTaskService, loggerFactory)
    {
    }
}