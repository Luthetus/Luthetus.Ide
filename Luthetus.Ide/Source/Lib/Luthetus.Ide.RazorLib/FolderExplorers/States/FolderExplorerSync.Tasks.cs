using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using static Luthetus.Ide.RazorLib.FolderExplorers.States.FolderExplorerState;

namespace Luthetus.Ide.RazorLib.FolderExplorers.States;

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
        Dispatcher.Dispatch(new WithAction(inFolderExplorerState => inFolderExplorerState with
        {
            IsLoadingFolderExplorer = true
        }));

        var rootNode = new TreeViewAbsolutePath(
            folderAbsolutePath,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            true,
            true);

        await rootNode.LoadChildListAsync();

        if (!_treeViewService.TryGetTreeViewContainer(
                TreeViewContentStateKey,
                out var treeViewState))
        {
            _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                TreeViewContentStateKey,
                rootNode,
                new TreeViewNoType[] { rootNode }.ToImmutableList()));
        }
        else
        {
            _treeViewService.SetRoot(TreeViewContentStateKey, rootNode);
            _treeViewService.SetActiveNode(TreeViewContentStateKey, rootNode);
        }

        Dispatcher.Dispatch(new WithAction(inFolderExplorerState => inFolderExplorerState with
        {
            IsLoadingFolderExplorer = false
        }));
    }
}