using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Keys;
using Luthetus.Ide.ClassLib.Store.SemanticContextCase;
using Luthetus.Common.RazorLib.BackgroundTaskCase;
using Luthetus.Ide.ClassLib.CompilerServices.ParserTaskCase;

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
        private readonly IParserTaskQueue _parserTaskQueue;

        public Effector(
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
            ITreeViewService treeViewService,
            IState<DotNetSolutionState> dotNetSolutionStateWrap,
            IParserTaskQueue parserTaskQueue)
        {
            _fileSystemProvider = fileSystemProvider;
            _environmentProvider = environmentProvider;
            _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
            _treeViewService = treeViewService;
            _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
            _parserTaskQueue = parserTaskQueue;
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
        public async Task HandleParseDotNetSolutionAction(
            IDispatcher dispatcher)
        {
            var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

            if (dotNetSolutionState.DotNetSolution is null)
                return;

            var dotNetSolutionSemanticContext = new DotNetSolutionSemanticContext(
                DotNetSolutionKey.NewSolutionKey(),
                dotNetSolutionState.DotNetSolution);

            dispatcher.Dispatch(
                new SemanticContextState.SetDotNetSolutionSemanticContextAction(
                    dotNetSolutionSemanticContext));

            var parserTask = new ParserTask(
                async cancellationToken =>
                {
                    dispatcher.Dispatch(new WithAction(inDotNetSolutionState =>
                        inDotNetSolutionState with
                        {
                            IsExecutingAsyncTaskLinks = inDotNetSolutionState.IsExecutingAsyncTaskLinks + 1
                        }));

                    await Task.Delay(5_000);

                    dispatcher.Dispatch(new WithAction(inDotNetSolutionState =>
                        inDotNetSolutionState with
                        {
                            IsExecutingAsyncTaskLinks = inDotNetSolutionState.IsExecutingAsyncTaskLinks - 1
                        }));
                },
                "Parse Task Name",
                "Parse Task Description",
                true,
                _ => Task.CompletedTask,
                dispatcher,
                CancellationToken.None);

            _parserTaskQueue.QueueParserWorkItem(parserTask);
        }
    }
}