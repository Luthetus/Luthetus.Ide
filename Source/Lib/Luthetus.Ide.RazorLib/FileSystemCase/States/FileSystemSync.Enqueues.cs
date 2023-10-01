using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;

namespace Luthetus.Ide.RazorLib.FileSystemCase.States;

public partial class FileSystemSync
{
    public void SaveFile(
        IAbsolutePath absolutePath,
        string content,
        Action<DateTime?> onAfterSaveCompletedWrittenDateTimeAction,
        CancellationToken cancellationToken = default)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Save File",
            async () => await SaveFileAsync(
                absolutePath,
                content,
                onAfterSaveCompletedWrittenDateTimeAction,
                cancellationToken));
    }
}