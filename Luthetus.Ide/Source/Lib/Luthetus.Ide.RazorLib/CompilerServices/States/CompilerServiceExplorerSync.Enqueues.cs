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

                var xmlCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.XmlCompilerService, _compilerServiceRegistry.XmlCompilerService.GetType(), "XML", true);

                var dotNetSolutionCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.DotNetSolutionCompilerService, _compilerServiceRegistry.DotNetSolutionCompilerService.GetType(), ".NET Solution", true);

                var cSharpProjectCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.CSharpProjectCompilerService, _compilerServiceRegistry.CSharpProjectCompilerService.GetType(), "C# Project", true);

                var cSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.CSharpCompilerService, _compilerServiceRegistry.CSharpCompilerService.GetType(), "C#", true);

                var razorCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.RazorCompilerService, _compilerServiceRegistry.RazorCompilerService.GetType(), "Razor", true);

                var cssCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.CssCompilerService, _compilerServiceRegistry.CssCompilerService.GetType(), "Css", true);

                var fSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.FSharpCompilerService, _compilerServiceRegistry.FSharpCompilerService.GetType(), "F#", true);

                var javaScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.JavaScriptCompilerService, _compilerServiceRegistry.JavaScriptCompilerService.GetType(), "JavaScript", true);

                var typeScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.TypeScriptCompilerService, _compilerServiceRegistry.TypeScriptCompilerService.GetType(), "TypeScript", true);

                var jsonCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.JsonCompilerService, _compilerServiceRegistry.JsonCompilerService.GetType(), "JSON", true);

                var rootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
                    new TreeViewReflectionWithView(xmlCompilerServiceWatchWindowObject, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                    new TreeViewReflectionWithView(dotNetSolutionCompilerServiceWatchWindowObject, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                    new TreeViewReflectionWithView(cSharpProjectCompilerServiceWatchWindowObject, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                    new TreeViewReflectionWithView(cSharpCompilerServiceWatchWindowObject, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                    new TreeViewReflectionWithView(razorCompilerServiceWatchWindowObject, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                    new TreeViewReflectionWithView(cssCompilerServiceWatchWindowObject, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                    new TreeViewReflectionWithView(fSharpCompilerServiceWatchWindowObject, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                    new TreeViewReflectionWithView(javaScriptCompilerServiceWatchWindowObject, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                    new TreeViewReflectionWithView(typeScriptCompilerServiceWatchWindowObject, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers),
                    new TreeViewReflectionWithView(jsonCompilerServiceWatchWindowObject, true, false, _luthetusIdeComponentRenderers, _luthetusCommonComponentRenderers));

                await rootNode.LoadChildListAsync();

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
                        rootNode,
						true,
						false);
                }

                Dispatcher.Dispatch(new NewAction(inCompilerServiceExplorerState =>
                    new CompilerServiceExplorerState(inCompilerServiceExplorerState.Model)));
            });
    }
}