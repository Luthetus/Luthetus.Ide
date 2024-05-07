using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.FileSystems.States;

public partial class FileSystemSync
{
    public Task SaveFile(
        IAbsolutePath absolutePath,
        string content,
        Action<DateTime?> onAfterSaveCompletedWrittenDateTimeAction,
        CancellationToken cancellationToken = default)
    {
        return BackgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Save File",
            async () => await SaveFileAsync(
                absolutePath,
                content,
                onAfterSaveCompletedWrittenDateTimeAction,
                cancellationToken));
    }
}