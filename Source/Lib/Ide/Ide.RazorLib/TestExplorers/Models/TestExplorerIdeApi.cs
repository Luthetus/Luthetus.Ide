using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.TestExplorers.States;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public class TestExplorerIdeApi
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
	private readonly IState<TerminalState> _terminalStateWrap;
	private readonly IState<TestExplorerState> _testExplorerStateWrap;
    private readonly IDispatcher _dispatcher;

    public TestExplorerIdeApi(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        DotNetCliOutputParser dotNetCliOutputParser,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        IState<TerminalState> terminalStateWrap,
        IState<TestExplorerState> testExplorerStateWrap,
        IDispatcher dispatcher)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
		_textEditorService = textEditorService;
		_backgroundTaskService = backgroundTaskService;
		_dotNetCliOutputParser = dotNetCliOutputParser;
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
		_terminalStateWrap = terminalStateWrap;
		_testExplorerStateWrap = testExplorerStateWrap;
        _dispatcher = dispatcher;
    }

    public void ConstructTreeView()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Refresh TestExplorer",
            ConstructTreeViewAsync);
    }

    private Task ConstructTreeViewAsync()
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
                    OutputParser: _dotNetCliOutputParser,
					ContinueWith: async () =>
                    {
						try
						{
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

                treeViewProjectTestModel.Item.TerminalCommand = dotNetTestListTestsCommand;

				executionTerminal.EnqueueCommand(dotNetTestListTestsCommand);
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
    
    public void DiscoverTests()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "DiscoverTests",
            DiscoverTestsAsync);
    }
    
    private Task DiscoverTestsAsync()
    {
    	var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

        if (dotNetSolutionModel is null)
            return Task.CompletedTask;
    	
    	var localTestExplorerState = _testExplorerStateWrap.Value;
    
    	var progressBarModel = new ProgressBarModel(0, $"Discovering tests in: {dotNetSolutionModel.AbsolutePath.NameWithExtension}");

		NotificationHelper.DispatchProgress(
	        "DiscoverTestsAsync",
	        progressBarModel,
	        _commonComponentRenderers,
	        _dispatcher,
	        TimeSpan.FromMilliseconds(-1));

		_ = Task.Run(async () =>
    	{
    		try
    		{
    			var completionPercentPerProject = 1.0 / (double)localTestExplorerState.ProjectTestModelList.Count;
	    		var projectsHandled = 0;
	    		
	    		if (_treeViewService.TryGetTreeViewContainer(TestExplorerState.TreeViewTestExplorerKey, out var treeViewContainer))
		        {
		        	if (treeViewContainer.RootNode is not TreeViewAdhoc treeViewAdhoc)
		        		return;
		        		
		            foreach (var treeViewProject in treeViewAdhoc.ChildList)
		            {
		            	if (treeViewProject is not TreeViewProjectTestModel treeViewProjectTestModel)
		            		return;
		            
		            	await treeViewProject.LoadChildListAsync();
		            	
		            	await Task.Delay(1_000);
			    		progressBarModel.SetProgress(
			    			projectsHandled * completionPercentPerProject,
			    			$"{projectsHandled + 1}/{localTestExplorerState.ProjectTestModelList.Count}: {treeViewProjectTestModel.Item.AbsolutePath.NameWithExtension}");
			    		projectsHandled++;
		            }
		        }
		    	
		    	progressBarModel.SetProgress(1, $"Finished discovering tests in: {dotNetSolutionModel.AbsolutePath.NameWithExtension}", string.Empty);
    		}
    		catch (Exception e)
			{
				var currentProgress = progressBarModel.GetProgress();
				progressBarModel.SetProgress(currentProgress, e.ToString());
			}
			finally
			{
				progressBarModel.Dispose();
			}
    	});
    	
    	return Task.CompletedTask;
    }
}
