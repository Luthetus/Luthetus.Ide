using Luthetus.Common.RazorLib.TreeView.Models.TreeViewClasses;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.WatchWindow;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States.CompilerServiceExplorerState;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;

public partial class CompilerServiceExplorerSync
{
    public async Task SetCompilerServiceExplorerTreeView()
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

        Dispatcher.Dispatch(new NewAction(inCompilerServiceExplorerState =>
            new CompilerServiceExplorerState(
                inCompilerServiceExplorerState.Model,
                inCompilerServiceExplorerState.GraphicalView,
                inCompilerServiceExplorerState.ReflectionView)));
    }
}