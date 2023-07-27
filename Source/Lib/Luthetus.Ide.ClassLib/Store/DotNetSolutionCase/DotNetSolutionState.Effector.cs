using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Common.RazorLib.TreeView;
using Fluxor;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.CompilerServices.Lang.DotNetSolution;

namespace Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;

public partial record DotNetSolutionState
{
    private class Effector
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
        private readonly ITreeViewService _treeViewService;
        private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
        private readonly ICompilerServiceBackgroundTaskQueue _compilerServiceBackgroundTaskQueue;

        public Effector(
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
            ITreeViewService treeViewService,
            IState<DotNetSolutionState> dotNetSolutionStateWrap,
            ICompilerServiceBackgroundTaskQueue compilerServiceBackgroundTaskQueue)
        {
            _fileSystemProvider = fileSystemProvider;
            _environmentProvider = environmentProvider;
            _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
            _treeViewService = treeViewService;
            _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
            _compilerServiceBackgroundTaskQueue = compilerServiceBackgroundTaskQueue;
        }

        [EffectMethod]
        public async Task HandleSetDotNetSolutionAction(
            SetDotNetSolutionAction setDotNetSolutionAction,
            IDispatcher dispatcher)
        {
            var dotNetSolutionAbsoluteFilePathString = setDotNetSolutionAction.SolutionAbsoluteFilePath.GetAbsoluteFilePathString();

            var content = await _fileSystemProvider.File.ReadAllTextAsync(
                dotNetSolutionAbsoluteFilePathString,
                CancellationToken.None);

            var solutionAbsoluteFilePath = new AbsoluteFilePath(
                dotNetSolutionAbsoluteFilePathString,
                false,
                _environmentProvider);

            var solutionNamespacePath = new NamespacePath(
                string.Empty,
                solutionAbsoluteFilePath);

            var dotNetSolution = DotNetSolutionParser.Parse(
                content,
                solutionNamespacePath,
                _environmentProvider);

            dispatcher.Dispatch(
                new WithAction(
                    inDotNetSolutionState => inDotNetSolutionState with
                    {
                        DotNetSolution = dotNetSolution
                    }));

            dispatcher.Dispatch(new SetDotNetSolutionTreeViewAction());
            dispatcher.Dispatch(new ParseDotNetSolutionAction());
        }

        [EffectMethod(typeof(SetDotNetSolutionTreeViewAction))]
        public async Task HandleSetDotNetSolutionTreeViewAction(
            IDispatcher dispatcher)
        {
            var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

            if (dotNetSolutionState.DotNetSolution is null)
                return;

            dispatcher.Dispatch(new WithAction(inDotNetSolutionState =>
                inDotNetSolutionState with
                {
                    IsExecutingAsyncTaskLinks = inDotNetSolutionState.IsExecutingAsyncTaskLinks + 1
                }));

            var rootNode = new TreeViewSolution(
                dotNetSolutionState.DotNetSolution,
                _luthetusIdeComponentRenderers,
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

            dispatcher.Dispatch(new WithAction(inDotNetSolutionState =>
                inDotNetSolutionState with
                {
                    IsExecutingAsyncTaskLinks = inDotNetSolutionState.IsExecutingAsyncTaskLinks - 1
                }));
        }

        [EffectMethod(typeof(ParseDotNetSolutionAction))]
        public Task HandleParseDotNetSolutionAction(
            IDispatcher dispatcher)
        {
            var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

            if (dotNetSolutionState.DotNetSolution is null)
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
    }
}