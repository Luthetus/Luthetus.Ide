using Luthetus.Common.RazorLib.TreeView;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Css;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.FSharp;
using Luthetus.CompilerServices.Lang.JavaScript;
using Luthetus.CompilerServices.Lang.Json;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.TypeScript;
using Luthetus.CompilerServices.Lang.Xml;
using Luthetus.Common.RazorLib.WatchWindow;
using Luthetus.Ide.RazorLib.ComponentRenderersCase;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.CompilerServiceExplorerCase;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase;

public partial class CompilerServiceExplorerRegistry
{
    private class Effector
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
        private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
        private readonly ITreeViewService _treeViewService;
        private readonly IState<CompilerServiceExplorerRegistry> _compilerServiceExplorerStateWrap;
        private readonly XmlCompilerService _xmlCompilerService;
        private readonly DotNetSolutionCompilerService _dotNetSolutionCompilerService;
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
            IState<CompilerServiceExplorerRegistry> compilerServiceExplorerStateWrap,
            XmlCompilerService xmlCompilerService,
            DotNetSolutionCompilerService dotNetSolutionCompilerService,
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
            _dotNetSolutionCompilerService = dotNetSolutionCompilerService;
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

            var xmlCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                _xmlCompilerService, _xmlCompilerService.GetType(), "XML", true);

            var dotNetSolutionCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                _dotNetSolutionCompilerService, _dotNetSolutionCompilerService.GetType(), ".NET Solution", true);

            var cSharpProjectCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                _cSharpProjectCompilerService, _cSharpProjectCompilerService.GetType(), "C# Project", true);

            var cSharpCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                _cSharpCompilerService, _cSharpCompilerService.GetType(), "C#", true);

            var razorCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                _razorCompilerService, _razorCompilerService.GetType(), "Razor", true);

            var cssCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                _cssCompilerService, _cssCompilerService.GetType(), "Css", true);

            var fSharpCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                _fSharpCompilerService, _fSharpCompilerService.GetType(), "F#", true);

            var javaScriptCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                _javaScriptCompilerService, _javaScriptCompilerService.GetType(), "JavaScript", true);

            var typeScriptCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                _typeScriptCompilerService, _typeScriptCompilerService.GetType(), "TypeScript", true);

            var jsonCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                _jsonCompilerService, _jsonCompilerService.GetType(), "JSON", true);

            var rootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
                new TreeViewReflectionWithView(xmlCompilerServiceWatchWindowObjectWrap, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                new TreeViewReflectionWithView(dotNetSolutionCompilerServiceWatchWindowObjectWrap, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                new TreeViewReflectionWithView(cSharpProjectCompilerServiceWatchWindowObjectWrap, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                new TreeViewReflectionWithView(cSharpCompilerServiceWatchWindowObjectWrap, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                new TreeViewReflectionWithView(razorCompilerServiceWatchWindowObjectWrap, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                new TreeViewReflectionWithView(cssCompilerServiceWatchWindowObjectWrap, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                new TreeViewReflectionWithView(fSharpCompilerServiceWatchWindowObjectWrap, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                new TreeViewReflectionWithView(javaScriptCompilerServiceWatchWindowObjectWrap, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                new TreeViewReflectionWithView(typeScriptCompilerServiceWatchWindowObjectWrap, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                new TreeViewReflectionWithView(jsonCompilerServiceWatchWindowObjectWrap, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers));

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

            dispatcher.Dispatch(new NewAction(inCompilerServiceExplorerState =>
                new CompilerServiceExplorerRegistry(
                    inCompilerServiceExplorerState.Model,
                    inCompilerServiceExplorerState.GraphicalView,
                    inCompilerServiceExplorerState.ReflectionView)));
        }
    }
}