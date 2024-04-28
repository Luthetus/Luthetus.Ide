using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Gits.States;

public partial class GitSync
{
    public void RefreshGit(CancellationToken cancellationToken)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Git Refresh",
            async () => await RefreshGitAsync(cancellationToken));
    }

    public void GitInit(CancellationToken cancellationToken)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Git Init",
            async () => await GitInitAsync(cancellationToken));
    }

    public void TryFindGitFolderInDirectory(
        IAbsolutePath directoryAbsolutePath,
        CancellationToken cancellationToken)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Git Find '.git' Folder",
            async () => await TryFindGitFolderInDirectoryAsync(directoryAbsolutePath, cancellationToken));
    }
}
