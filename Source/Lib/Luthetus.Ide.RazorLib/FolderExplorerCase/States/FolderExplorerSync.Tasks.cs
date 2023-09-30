using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using static Luthetus.Ide.RazorLib.FolderExplorerCase.States.FolderExplorerState;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase.States;

public partial class FolderExplorerSync
{
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