using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.Logging;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// <summary>
/// The <see cref="BlockingBackgroundTaskWorker"/> should be assumed to never
/// be immediately available, that one always will wait in a queue for a user-noticeable
/// amount of time.
/// </summary>
public class BlockingBackgroundTaskWorker : BackgroundTaskWorker
{
    public static BackgroundTaskQueue Queue { get; } = new BackgroundTaskQueue(
        Key<BackgroundTaskQueue>.NewKey(),
        "BlockingBackgroundTaskWorker");

    public BlockingBackgroundTaskWorker(
            Key<BackgroundTaskQueue> queueKey,
            IBackgroundTaskService backgroundTaskService,
            ILoggerFactory loggerFactory)
        : base(queueKey, backgroundTaskService, loggerFactory)
    {
    }
}
