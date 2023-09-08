using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Common.RazorLib.TreeView;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations.CompilerServiceExplorerCase;

namespace Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;

public partial record CompilerServiceExplorerState
{
    private class Effector
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
        private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
        private readonly ITreeViewService _treeViewService;
        private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
        private readonly CSharpCompilerService _cSharpCompilerService;

        public Effector(
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
            ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
            ITreeViewService treeViewService,
            IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
            CSharpCompilerService cSharpCompilerService)
        {
            _fileSystemProvider = fileSystemProvider;
            _environmentProvider = environmentProvider;
            _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
            _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
            _treeViewService = treeViewService;
            _compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
            _cSharpCompilerService = cSharpCompilerService;
        }

        [EffectMethod]
        public Task HandleSetCompilerServiceExplorerAction(
            SetCompilerServiceExplorerAction setCompilerServiceExplorerAction,
            IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new SetCompilerServiceExplorerTreeViewAction());

            return Task.CompletedTask;
        }

        [EffectMethod(typeof(SetCompilerServiceExplorerTreeViewAction))]
        public async Task HandleSetCompilerServiceExplorerTreeViewAction(
            IDispatcher dispatcher)
        {
            var compilerServiceExplorerState = _compilerServiceExplorerStateWrap.Value;

            var compilerServicesExplorerRoot = new CompilerServicesExplorerRoot(_cSharpCompilerService);

            var rootNode = new TreeViewCompilerServicesExplorerRoot(
                compilerServicesExplorerRoot,
                _luthetusIdeComponentRenderers,
                _luthetusCommonComponentRenderers,
                _fileSystemProvider,
                _environmentProvider,
                true,
                true);

            await rootNode.LoadChildrenAsync();

            if (!_treeViewService.TryGetTreeViewState(
                    TreeViewCompilerServiceExplorerContentStateKey,
                    out var treeViewState))
            {
                _treeViewService.RegisterTreeViewState(new TreeViewState(
                    TreeViewCompilerServiceExplorerContentStateKey,
                    rootNode,
                    rootNode,
                    ImmutableList<TreeViewNoType>.Empty));
            }
            else
            {
                _treeViewService.SetRoot(
                    TreeViewCompilerServiceExplorerContentStateKey,
                    rootNode);

                _treeViewService.SetActiveNode(
                    TreeViewCompilerServiceExplorerContentStateKey,
                    rootNode);
            }

            dispatcher.Dispatch(new WithAction(inCompilerServiceExplorerState =>
                inCompilerServiceExplorerState with
                {
                    IsLoadingCompilerServiceExplorer = false
                }));
        }
    }
}