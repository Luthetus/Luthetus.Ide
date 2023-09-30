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
            "SetDotNetSolutionAsync",
            async () => await SetFolderExplorerAsync(folderAbsolutePath));
    }

    public void SetFolderExplorerTreeView(IAbsolutePath folderAbsolutePath)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "SetDotNetSolutionAsync",
            async () => await SetFolderExplorerTreeViewAsync(folderAbsolutePath));
    }

    public void ShowInputFile()
    {
        Dispatcher.Dispatch(new InputFileState.RequestInputFileStateFormAction(
            InputFileSync,
            "Folder Explorer",
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
            }.ToImmutableArray()));
    }

    private async Task SetFolderExplorerAsync(IAbsolutePath folderAbsolutePath)
    {
        Dispatcher.Dispatch(new WithAction(
            inFolderExplorerState => inFolderExplorerState with
            {
                AbsolutePath = folderAbsolutePath
            }));

        await SetFolderExplorerTreeViewAsync(folderAbsolutePath);
    }

    private async Task SetFolderExplorerTreeViewAsync(IAbsolutePath folderAbsolutePath)
    {
        Dispatcher.Dispatch(new WithAction(inFolderExplorerState =>
            inFolderExplorerState with
            {
                IsLoadingFolderExplorer = true
            }));

        var rootNode = new TreeViewAbsolutePath(
            folderAbsolutePath,
            _luthetusIdeComponentRenderers,
            _luthetusCommonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            true,
            true);

        await rootNode.LoadChildrenAsync();

        if (!_treeViewService.TryGetTreeViewState(
                TreeViewFolderExplorerContentStateKey,
                out var treeViewState))
        {
            _treeViewService.RegisterTreeViewState(new TreeViewContainer(
                TreeViewFolderExplorerContentStateKey,
                rootNode,
                rootNode,
                ImmutableList<TreeViewNoType>.Empty));
        }
        else
        {
            _treeViewService.SetRoot(
                TreeViewFolderExplorerContentStateKey,
                rootNode);

            _treeViewService.SetActiveNode(
                TreeViewFolderExplorerContentStateKey,
                rootNode);
        }

        Dispatcher.Dispatch(new WithAction(inFolderExplorerState =>
            inFolderExplorerState with
            {
                IsLoadingFolderExplorer = false
            }));
    }
}