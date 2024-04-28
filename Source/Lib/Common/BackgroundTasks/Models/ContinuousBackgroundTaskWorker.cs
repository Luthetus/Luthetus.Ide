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
    public const string QUEUE_DISPLAY_NAME = "ContinuousBackgroundTaskWorker";

    private static readonly Key<BackgroundTaskQueue> _queueKey = new(Guid.Parse("78912ee9-1b3f-4bc3-ab8b-5681fbf0b131"));

    public ContinuousBackgroundTaskWorker(
            IBackgroundTaskService backgroundTaskService,
            ILoggerFactory loggerFactory)
        : base(_queueKey, backgroundTaskService, loggerFactory)
    {
    }

    public static Key<BackgroundTaskQueue> GetQueueKey() => _queueKey;
}