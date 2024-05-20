using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.TestExplorers.States;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public class LuthetusIdeTestExplorerBackgroundTaskApi
{
    private readonly LuthetusIdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly ITextEditorService _textEditorService;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public LuthetusIdeTestExplorerBackgroundTaskApi(
        LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
		DotNetCliOutputParser dotNetCliOutputParser,
        IState<TerminalState> terminalStateWrap,
        IDispatcher dispatcher)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
		_textEditorService = textEditorService;
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_terminalStateWrap = terminalStateWrap;
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;
    }

    public Task DotNetSolutionStateWrap_StateChanged()
    {
        return _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Refresh TestExplorer",
            async () => await DotNetSolutionStateWrap_StateChangedAsync().ConfigureAwait(false));
    }

    private Task DotNetSolutionStateWrap_StateChangedAsync()
    {
        var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

        if (dotNetSolutionModel is null)
            return Task.CompletedTask;

        var localDotNetProjectList = dotNetSolutionModel.DotNetProjectList
            .Where(x => x.DotNetProjectKind == DotNetProjectKind.CSharpProject);

        var localProjectTestModelList = localDotNetProjectList.Select(x => new ProjectTestModel(
            x.ProjectIdGuid,
            x.AbsolutePath,
            callback => Task.CompletedTask,
            node => _treeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, node)));

        var localFormattedCommand = DotNetCliCommandFormatter.FormatDotNetTestListTests();

        var localTreeViewProjectTestModelList = localProjectTestModelList.Select(x =>
                (TreeViewNoType)new TreeViewProjectTestModel(
                    x,
                    _commonComponentRenderers,
                    true,
                    false))
            .ToArray();

        foreach (var entry in localTreeViewProjectTestModelList)
        {
            var treeViewProjectTestModel = (TreeViewProjectTestModel)entry;

            if (string.IsNullOrWhiteSpace(treeViewProjectTestModel.Item.DirectoryNameForTestDiscovery))
                return Task.CompletedTask;

            treeViewProjectTestModel.Item.EnqueueDiscoverTestsFunc = async callback =>
            {
                var executionTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_TERMINAL_KEY];

                var dotNetTestListTestsCommand = new TerminalCommand(
                    treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey,
                    localFormattedCommand,
                    treeViewProjectTestModel.Item.DirectoryNameForTestDiscovery,
                    CancellationToken.None,
                    async () =>
                    {
						await _textEditorService.PostSimpleBatch(
	                        "LoadTestExplorer",
							"LoadTestExplorer",
	                        async editContext =>
							{
								try
		                        {
		                            var success = executionTerminal.TryGetTerminalCommandTextSpan(
		                                treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey,
		                                out var terminalCommandTextSpan);
		
		                            var output = terminalCommandTextSpan?.GetText();
		                            if (output is null)
		                                return;
		
		                            treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput =
		                                _dotNetCliOutputParser.TheFollowingTestsAreAvailableList ?? new();
		
		                            // THINKING_ABOUT_TREE_VIEW();
		                            {
		                                var splitOutputList = treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput
		                                    .Select(x => x.Split('.'));
		
		                                var rootMap = new Dictionary<string, StringFragment>();
		
		                                foreach (var splitOutput in splitOutputList)
		                                {
		                                    var targetMap = rootMap;
		                                    var lastSeenStringFragment = (StringFragment?)null;
		
		                                    foreach (var fragment in splitOutput)
		                                    {
		                                        if (!targetMap.ContainsKey(fragment))
		                                            targetMap.Add(fragment, new(fragment));
		
		                                        lastSeenStringFragment = targetMap[fragment];
		                                        targetMap = lastSeenStringFragment.Map;
		                                    }
		
		                                    if (lastSeenStringFragment is not null)
		                                        lastSeenStringFragment.IsEndpoint = true;
		                                }
		
		                                treeViewProjectTestModel.Item.RootStringFragmentMap = rootMap;
		                                await callback.Invoke(rootMap).ConfigureAwait(false);
		                            }
		                        }
								catch (Exception)
		                        {
		                            await callback.Invoke(new()).ConfigureAwait(false);
		                            throw;
		                        }
							});
                    },
                    () =>
                    {
                        // Should the 'ClearStandardOut(...)' logic still be here? (2024-04-28)
                        //
                        //executionTerminal.ClearStandardOut(
                        //    treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey);

                        return Task.CompletedTask;
                    });

                await executionTerminal.EnqueueCommandAsync(dotNetTestListTestsCommand).ConfigureAwait(false);
            };
        }

        var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(localTreeViewProjectTestModelList);
        var firstNode = localTreeViewProjectTestModelList.FirstOrDefault();

        var activeNodes = firstNode is null
            ? Array.Empty<TreeViewNoType>()
            : new[] { firstNode };

        if (!_treeViewService.TryGetTreeViewContainer(TestExplorerState.TreeViewTestExplorerKey, out _))
        {
            _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                TestExplorerState.TreeViewTestExplorerKey,
                adhocRoot,
                activeNodes.ToImmutableList()));
        }
        else
        {
            _treeViewService.SetRoot(TestExplorerState.TreeViewTestExplorerKey, adhocRoot);

            _treeViewService.SetActiveNode(
                TestExplorerState.TreeViewTestExplorerKey,
                firstNode,
                true,
                false);
        }

        _dispatcher.Dispatch(new TestExplorerState.WithAction(inState => inState with
        {
            ProjectTestModelList = localProjectTestModelList.ToImmutableList()
        }));

        return Task.CompletedTask;
    }
}
