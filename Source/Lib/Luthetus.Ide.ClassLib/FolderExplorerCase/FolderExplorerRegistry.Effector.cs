using Luthetus.Ide.ClassLib.ComponentRenderersCase;
using Luthetus.Ide.ClassLib.TreeViewImplementationsCase;
using Luthetus.Common.RazorLib.TreeView;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.ComponentRenderers;

namespace Luthetus.Ide.ClassLib.FolderExplorerCase;

public partial record FolderExplorerRegistry
{
    private class Effector
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
        private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
        private readonly ITreeViewService _treeViewService;
        private readonly IState<FolderExplorerRegistry> _folderExplorerStateWrap;

        public Effector(
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
            ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
            ITreeViewService treeViewService,
            IState<FolderExplorerRegistry> folderExplorerStateWrap)
        {
            _fileSystemProvider = fileSystemProvider;
            _environmentProvider = environmentProvider;
            _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
            _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
            _treeViewService = treeViewService;
            _folderExplorerStateWrap = folderExplorerStateWrap;
        }

        [EffectMethod]
        public Task HandleSetFolderExplorerAction(
            SetFolderExplorerAction setFolderExplorerAction,
            IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new WithAction(
                inFolderExplorerState => inFolderExplorerState with
                {
                    AbsolutePath = setFolderExplorerAction.FolderAbsolutePath
                }));

            dispatcher.Dispatch(new SetFolderExplorerTreeViewAction());

            return Task.CompletedTask;
        }

        [EffectMethod(typeof(SetFolderExplorerTreeViewAction))]
        public async Task HandleSetFolderExplorerTreeViewAction(
            IDispatcher dispatcher)
        {
            var folderExplorerState = _folderExplorerStateWrap.Value;

            if (folderExplorerState.AbsolutePath is null)
                return;

            dispatcher.Dispatch(new WithAction(inFolderExplorerState =>
                inFolderExplorerState with
                {
                    IsLoadingFolderExplorer = true
                }));

            var rootNode = new TreeViewAbsolutePath(
                folderExplorerState.AbsolutePath,
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