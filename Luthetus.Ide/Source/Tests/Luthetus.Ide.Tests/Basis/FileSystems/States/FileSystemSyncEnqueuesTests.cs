using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.Tests.Basis.FileSystems.States;

public class FileSystemSyncEnqueuesTests
{
    public void SaveFile(
        IAbsolutePath absolutePath,
        string content,
        Action<DateTime?> onAfterSaveCompletedWrittenDateTimeAction,
        CancellationToken cancellationToken = default)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Save File",
            async () => await SaveFileAsync(
                absolutePath,
                content,
                onAfterSaveCompletedWrittenDateTimeAction,
                cancellationToken));
    }
}