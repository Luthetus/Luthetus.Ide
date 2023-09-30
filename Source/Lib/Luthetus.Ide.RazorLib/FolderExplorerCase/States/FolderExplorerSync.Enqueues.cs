using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.States;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using static Luthetus.Ide.RazorLib.FolderExplorerCase.States.FolderExplorerState;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase.States;

public partial class FolderExplorerSync
{
    public void SetFolderExplorerState(IAbsolutePath folderAbsolutePath)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Set FolderExplorer State",
            async () => await SetFolderExplorerAsync(folderAbsolutePath));
    }

    public void SetFolderExplorerTreeView(IAbsolutePath folderAbsolutePath)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Set FolderExplorer TreeView",
            async () => await SetFolderExplorerTreeViewAsync(folderAbsolutePath));
    }

    public void ShowInputFile()
    {
        InputFileSync.RequestInputFileStateForm("Folder Explorer",
            async afp =>
            {
                if (afp is not null)
                    await SetFolderExplorerAsync(afp);
            },
            afp =>
            {
                if (afp is null || !afp.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", afp => afp.IsDirectory)
            }.ToImmutableArray());
    }
}