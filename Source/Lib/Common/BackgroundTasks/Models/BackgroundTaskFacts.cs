using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public static class BackgroundTaskFacts
{
	public static Key<IBackgroundTaskQueue> ContinuousQueueKey { get; } = new Key<IBackgroundTaskQueue>(Guid.Parse("78912ee9-1b3f-4bc3-ab8b-5681fbf0b131"));
	public static Key<IBackgroundTaskQueue> IndefiniteQueueKey { get; } = new Key<IBackgroundTaskQueue>(Guid.Parse("7905c763-c3fd-418e-b73d-4ca18666c20c"));
}
