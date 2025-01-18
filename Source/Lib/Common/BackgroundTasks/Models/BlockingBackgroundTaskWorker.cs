using Microsoft.Extensions.Logging;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// <summary>
/// The <see cref="BlockingBackgroundTaskWorker"/> should be assumed to never
/// be immediately available, that one always will wait in a queue for a user-noticeable
/// amount of time.
/// </summary>
public class BlockingBackgroundTaskWorker : BackgroundTaskWorker
{
    public const string QUEUE_DISPLAY_NAME = "BlockingBackgroundTaskWorker";

    private static readonly Key<IBackgroundTaskQueue> _queueKey = new(Guid.Parse("7905c763-c3fd-418e-b73d-4ca18666c20c"));

    public BlockingBackgroundTaskWorker(
            IBackgroundTaskService backgroundTaskService,
            ILoggerFactory loggerFactory,
        	LuthetusHostingKind luthetusHostingKind)
        : base(_queueKey, backgroundTaskService, loggerFactory, luthetusHostingKind)
    {
    }

    public static Key<IBackgroundTaskQueue> GetQueueKey() => _queueKey;
}
