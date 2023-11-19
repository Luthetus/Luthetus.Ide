using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.CompilerServices.States.CompilerServiceExplorerState;

namespace Luthetus.Ide.RazorLib.CompilerServices.States;

public partial class CompilerServiceExplorerSync
{
    public void SetCompilerServiceExplorerTreeView()
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set CompilerServiceExplorer TreeView",
            async () => 
            {
                var compilerServiceExplorerState = _compilerServiceExplorerStateWrap.Value;

                var xmlCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                    _compilerServiceRegistry.XmlCompilerService, _compilerServiceRegistry.XmlCompilerService.GetType(), "XML", true);

                var dotNetSolutionCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                    _compilerServiceRegistry.DotNetSolutionCompilerService, _compilerServiceRegistry.DotNetSolutionCompilerService.GetType(), ".NET Solution", true);

                var cSharpProjectCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                    _compilerServiceRegistry.CSharpProjectCompilerService, _compilerServiceRegistry.CSharpProjectCompilerService.GetType(), "C# Project", true);

                var cSharpCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                    _compilerServiceRegistry.CSharpCompilerService, _compilerServiceRegistry.CSharpCompilerService.GetType(), "C#", true);

                var razorCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                    _compilerServiceRegistry.RazorCompilerService, _compilerServiceRegistry.RazorCompilerService.GetType(), "Razor", true);

                var cssCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                    _compilerServiceRegistry.CssCompilerService, _compilerServiceRegistry.CssCompilerService.GetType(), "Css", true);

                var fSharpCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                    _compilerServiceRegistry.FSharpCompilerService, _compilerServiceRegistry.FSharpCompilerService.GetType(), "F#", true);

                var javaScriptCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                    _compilerServiceRegistry.JavaScriptCompilerService, _compilerServiceRegistry.JavaScriptCompilerService.GetType(), "JavaScript", true);

                var typeScriptCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                    _compilerServiceRegistry.TypeScriptCompilerService, _compilerServiceRegistry.TypeScriptCompilerService.GetType(), "TypeScript", true);

                var jsonCompilerServiceWatchWindowObjectWrap = new WatchWindowObjectWrap(
                    _compilerServiceRegistry.JsonCompilerService, _compilerServiceRegistry.JsonCompilerService.GetType(), "JSON", true);

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

                await rootNode.LoadChildBagAsync();

                if (!_treeViewService.TryGetTreeViewContainer(
                        TreeViewCompilerServiceExplorerContentStateKey,
                        out var treeViewState))
                {
                    _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                        TreeViewCompilerServiceExplorerContentStateKey,
                        rootNode,
                        new TreeViewNoType[] { rootNode }.ToImmutableList()));
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
                    new CompilerServiceExplorerState(inCompilerServiceExplorerState.Model)));
            });
    }
}