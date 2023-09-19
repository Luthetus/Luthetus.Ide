using System.Collections.Immutable;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using static Luthetus.Ide.RazorLib.FolderExplorerCase.States.FolderExplorerState;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase.States;

public partial class FolderExplorerSync
{
    public Task SetFolderExplorer(SetFolderExplorerAction setFolderExplorerAction)
    {
        Dispatcher.Dispatch(new WithAction(
            inFolderExplorerState => inFolderExplorerState with
            {
                AbsolutePath = setFolderExplorerAction.FolderAbsolutePath
            }));

        Dispatcher.Dispatch(new SetFolderExplorerTreeViewAction(
            this,
            setFolderExplorerAction.FolderAbsolutePath));

        return Task.CompletedTask;
    }

    public async Task SetFolderExplorerTreeView(SetFolderExplorerTreeViewAction inTask)
    {
        if (inTask.FolderAbsolutePath is null)
            return;

        Dispatcher.Dispatch(new WithAction(inFolderExplorerState =>
            inFolderExplorerState with
            {
                IsLoadingFolderExplorer = true
            }));

        var rootNode = new TreeViewAbsolutePath(
            inTask.FolderAbsolutePath,
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