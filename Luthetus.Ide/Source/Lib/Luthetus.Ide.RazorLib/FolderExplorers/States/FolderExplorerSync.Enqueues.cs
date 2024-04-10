using System.Collections.Immutable;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;

namespace Luthetus.Ide.RazorLib.FolderExplorers.States;

public partial class FolderExplorerSync
{
    public void SetFolderExplorerState(IAbsolutePath folderAbsolutePath)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set FolderExplorer State",
            async () => await SetFolderExplorerAsync(folderAbsolutePath));
    }

    public void SetFolderExplorerTreeView(IAbsolutePath folderAbsolutePath)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set FolderExplorer TreeView",
            async () => await SetFolderExplorerTreeViewAsync(folderAbsolutePath));
    }

    public Task ShowInputFile()
    {
        InputFileSync.RequestInputFileStateForm("Folder Explorer",
            async absolutePath =>
            {
                if (absolutePath is not null)
                    await SetFolderExplorerAsync(absolutePath);
            },
            absolutePath =>
            {
                if (absolutePath is null || !absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", absolutePath => absolutePath.IsDirectory)
            }.ToImmutableArray());

        return Task.CompletedTask;
    }
}