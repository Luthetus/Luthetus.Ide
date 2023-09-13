using Luthetus.Common.RazorLib.TreeView;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Ide.RazorLib.ComponentRenderersCase;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase;
using Luthetus.Ide.RazorLib.TerminalCase;
using Luthetus.Common.RazorLib;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase;
using static Luthetus.Ide.RazorLib.DotNetSolutionCase.States.DotNetSolutionState;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

internal partial class SynchronizationContext
{
    public async Task SetDotNetSolutionAsync(SetDotNetSolutionTask setDotNetSolutionAction)
    {
        var dotNetSolutionAbsolutePathString = setDotNetSolutionAction.SolutionAbsolutePath.FormattedInput;

        var content = await _fileSystemProvider.File.ReadAllTextAsync(
            dotNetSolutionAbsolutePathString,
            CancellationToken.None);

        var solutionAbsolutePath = new AbsolutePath(
            dotNetSolutionAbsolutePathString,
            false,
            _environmentProvider);

        var solutionNamespacePath = new NamespacePath(
            string.Empty,
            solutionAbsolutePath);

        var dotNetSolution = DotNetSolutionLexer.Lex(
            content,
            solutionNamespacePath,
            _environmentProvider);

        _dispatcher.Dispatch(new RegisterAction(dotNetSolution));

        _dispatcher.Dispatch(new WithAction(
            inDotNetSolutionState => inDotNetSolutionState with
            {
                DotNetSolutionModelKey = dotNetSolution.DotNetSolutionModelKey
            }));

        _dispatcher.Dispatch(new SetDotNetSolutionTreeViewTask());
        _dispatcher.Dispatch(new ParseDotNetSolutionTask());
    }

    public async Task<DotNetSolutionModel> SetDotNetSolutionTreeViewAsync(
        SetDotNetSolutionTreeViewTask setDotNetSolutionTreeViewAction)
    {
        var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutions.FirstOrDefault(x =>
            x.DotNetSolutionModelKey == dotNetSolutionState.DotNetSolutionModelKey);

        if (dotNetSolutionModel is null)
            return;

        _dispatcher.Dispatch(new WithAction(inDotNetSolutionState =>
            inDotNetSolutionState with
            {
                IsExecutingAsyncTaskLinks = inDotNetSolutionState.IsExecutingAsyncTaskLinks + 1
            }));

        var rootNode = new TreeViewSolution(
            dotNetSolutionModel,
            _luthetusIdeComponentRenderers,
            _luthetusCommonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            true,
            true);

        await rootNode.LoadChildrenAsync();

        if (!_treeViewService.TryGetTreeViewState(
                TreeViewSolutionExplorerStateKey,
                out _))
        {
            _treeViewService.RegisterTreeViewState(new TreeViewState(
                TreeViewSolutionExplorerStateKey,
                rootNode,
                rootNode,
                ImmutableList<TreeViewNoType>.Empty));
        }
        else
        {
            _treeViewService.SetRoot(
                TreeViewSolutionExplorerStateKey,
                rootNode);

            _treeViewService.SetActiveNode(
                TreeViewSolutionExplorerStateKey,
                rootNode);
        }

        _dispatcher.Dispatch(new WithAction(inDotNetSolutionState =>
            inDotNetSolutionState with
            {
                IsExecutingAsyncTaskLinks = inDotNetSolutionState.IsExecutingAsyncTaskLinks - 1
            }));
    }

    public Task ParseDotNetSolutionAsync(ParseDotNetSolutionTask parseDotNetSolutionAction)
    {
        var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutions.FirstOrDefault(x =>
            x.DotNetSolutionModelKey == dotNetSolutionState.DotNetSolutionModelKey);

        if (dotNetSolutionModel is null)
            return Task.CompletedTask;

        //var parserTask = new ParserTask(
        //    async cancellationToken =>
        //    {
        //        dispatcher.Dispatch(new WithAction(inDotNetSolutionState =>
        //            inDotNetSolutionState with
        //            {
        //                IsExecutingAsyncTaskLinks = inDotNetSolutionState.IsExecutingAsyncTaskLinks + 1
        //            }));

        //        await Task.Delay(5_000);

        //        dispatcher.Dispatch(new WithAction(inDotNetSolutionState =>
        //            inDotNetSolutionState with
        //            {
        //                IsExecutingAsyncTaskLinks = inDotNetSolutionState.IsExecutingAsyncTaskLinks - 1
        //            }));
        //    },
        //    "Parse Task Name",
        //    "Parse Task Description",
        //    false,
        //    _ => Task.CompletedTask,
        //    dispatcher,
        //    CancellationToken.None);

        //_parserTaskQueue.QueueParserWorkItem(parserTask);

        return Task.CompletedTask;
    }

    
    public Task AddExistingProjectToSolutionAsync(AddExistingProjectToSolutionAction addExistingProjectToSolutionAction)
    {
        var dotNetSolutionModel = _dotNetSolutionStateWrap.Value.DotNetSolutions.FirstOrDefault(
            x => x.DotNetSolutionModelKey == addExistingProjectToSolutionAction.DotNetSolutionModelKey);

        if (dotNetSolutionModel is null)
            return Task.CompletedTask;

        var projectTypeGuid = WebsiteProjectTemplateRegistry.GetProjectTypeGuid(
            addExistingProjectToSolutionAction.LocalProjectTemplateShortName);

        var relativePathFromSlnToProject = AbsolutePath.ConstructRelativePathFromTwoAbsolutePaths(
            dotNetSolutionModel.NamespacePath.AbsolutePath,
            addExistingProjectToSolutionAction.CSharpProjectAbsolutePath,
            addExistingProjectToSolutionAction.EnvironmentProvider);

        var projectIdGuid = Guid.NewGuid();

        var cSharpProject = new CSharpProject(
            addExistingProjectToSolutionAction.LocalCSharpProjectName,
            projectTypeGuid,
            relativePathFromSlnToProject,
            projectIdGuid);

        cSharpProject.SetAbsolutePath(addExistingProjectToSolutionAction.CSharpProjectAbsolutePath);

        var dotNetSolutionBuilder = dotNetSolutionModel.AddDotNetProject(
            cSharpProject,
            addExistingProjectToSolutionAction.EnvironmentProvider);

        var nextDotNetSolutions = inDotNetSolutionState.DotNetSolutions.SetItem(
            indexOfDotNetSolutionModel,
            dotNetSolutionBuilder.Build());

        return Task.CompletedTask;
    }
}