using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Common.RazorLib.TreeView;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.FolderExplorerCase;

public partial record FolderExplorerState
{
    private class Effector
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
        private readonly ITreeViewService _treeViewService;
        private readonly IState<FolderExplorerState> _folderExplorerStateWrap;

        public Effector(
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
            ITreeViewService treeViewService,
            IState<FolderExplorerState> folderExplorerStateWrap)
        {
            _fileSystemProvider = fileSystemProvider;
            _environmentProvider = environmentProvider;
            _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
            _treeViewService = treeViewService;
            _folderExplorerStateWrap = folderExplorerStateWrap;
        }

        [EffectMethod]
        public Task HandleSetFolderExplorerAction(
            SetFolderExplorerAction setFolderExplorerAction,
            IDispatcher dispatcher)
        {
            dispatcher.Dispatch(
                new WithAction(
                    inFolderExplorerState => inFolderExplorerState with
                    {
                        AbsoluteFilePath = setFolderExplorerAction.FolderAbsoluteFilePath
                    }));

            dispatcher.Dispatch(new SetFolderExplorerTreeViewAction());

            return Task.CompletedTask;
        }

        [EffectMethod(typeof(SetFolderExplorerTreeViewAction))]
        public async Task HandleSetFolderExplorerTreeViewAction(
            IDispatcher dispatcher)
        {
            var folderExplorerState = _folderExplorerStateWrap.Value;

            if (folderExplorerState.AbsoluteFilePath is null)
                return;

            dispatcher.Dispatch(new WithAction(inFolderExplorerState =>
                inFolderExplorerState with
                {
                    IsLoadingFolderExplorer = true
                }));

            var rootNode = new TreeViewAbsoluteFilePath(
                folderExplorerState.AbsoluteFilePath,
                _luthetusIdeComponentRenderers,
                _fileSystemProvider,
                _environmentProvider,
                true,
                true);

            await rootNode.LoadChildrenAsync();

            if (!_treeViewService.TryGetTreeViewState(
                    TreeViewFolderExplorerContentStateKey,
                    out var treeViewState))
            {
                _treeViewService.RegisterTreeViewState(new TreeViewState(
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

            dispatcher.Dispatch(new WithAction(inFolderExplorerState =>
                inFolderExplorerState with
                {
                    IsLoadingFolderExplorer = false
                }));
        }
    }
}