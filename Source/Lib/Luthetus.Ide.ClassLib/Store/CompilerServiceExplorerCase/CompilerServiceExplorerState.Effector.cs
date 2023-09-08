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
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Css;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.FSharp;
using Luthetus.CompilerServices.Lang.JavaScript;
using Luthetus.CompilerServices.Lang.Json;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.TypeScript;
using Luthetus.CompilerServices.Lang.Xml;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewClasses;
using Luthetus.Common.RazorLib.WatchWindow;

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
        private readonly XmlCompilerService _xmlCompilerService;
        private readonly DotNetSolutionCompilerService _dotNetCompilerService;
        private readonly CSharpProjectCompilerService _cSharpProjectCompilerService;
        private readonly CSharpCompilerService _cSharpCompilerService;
        private readonly RazorCompilerService _razorCompilerService;
        private readonly CssCompilerService _cssCompilerService;
        private readonly FSharpCompilerService _fSharpCompilerService;
        private readonly JavaScriptCompilerService _javaScriptCompilerService;
        private readonly TypeScriptCompilerService _typeScriptCompilerService;
        private readonly JsonCompilerService _jsonCompilerService;

        public Effector(
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
            ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
            ITreeViewService treeViewService,
            IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
            XmlCompilerService xmlCompilerService,
            DotNetSolutionCompilerService dotNetCompilerService,
            CSharpProjectCompilerService cSharpProjectCompilerService,
            CSharpCompilerService cSharpCompilerService,
            RazorCompilerService razorCompilerService,
            CssCompilerService cssCompilerService,
            FSharpCompilerService fSharpCompilerService,
            JavaScriptCompilerService javaScriptCompilerService,
            TypeScriptCompilerService typeScriptCompilerService,
            JsonCompilerService jsonCompilerService)
        {
            _fileSystemProvider = fileSystemProvider;
            _environmentProvider = environmentProvider;
            _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
            _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
            _treeViewService = treeViewService;
            _compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
            _xmlCompilerService = xmlCompilerService;
            _dotNetCompilerService = dotNetCompilerService;
            _cSharpProjectCompilerService = cSharpProjectCompilerService;
            _cSharpCompilerService = cSharpCompilerService;
            _razorCompilerService = razorCompilerService;
            _cssCompilerService = cssCompilerService;
            _fSharpCompilerService = fSharpCompilerService;
            _javaScriptCompilerService = javaScriptCompilerService;
            _typeScriptCompilerService = typeScriptCompilerService;
            _jsonCompilerService = jsonCompilerService;
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

            var compilerServiceCollection = new CompilerServiceCollection(
                _xmlCompilerService,
                _dotNetCompilerService,
                _cSharpProjectCompilerService,
                _cSharpCompilerService,
                _razorCompilerService,
                _cssCompilerService,
                _fSharpCompilerService,
                _javaScriptCompilerService,
                _typeScriptCompilerService,
                _jsonCompilerService);

            var watchWindowObjectWrap = new WatchWindowObjectWrap(
                compilerServiceCollection,
                compilerServiceCollection.GetType(),
                "Compiler Services",
                true);

            var rootNode = new TreeViewReflection(
                watchWindowObjectWrap,
                true,
                true,
                _luthetusCommonComponentRenderers.WatchWindowTreeViewRenderers!);

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