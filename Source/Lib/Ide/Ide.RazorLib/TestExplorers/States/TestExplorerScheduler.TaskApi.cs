using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.TestExplorers.States;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public partial class TestExplorerScheduler
{
    public Task Task_ConstructTreeView()
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

            treeViewProjectTestModel.Item.EnqueueDiscoverTestsFunc = callback =>
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

                return Task.CompletedTask;
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
    
    public Task Task_DiscoverTests()
    {
    	var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

        if (dotNetSolutionModel is null)
            return Task.CompletedTask;
    	
    	var localTestExplorerState = _testExplorerStateWrap.Value;
    
    	//var progressBarModel = new ProgressBarModel(0, $"Discovering tests in: {dotNetSolutionModel.AbsolutePath.NameWithExtension}");

		//NotificationHelper.DispatchProgress(
	    //    "DiscoverTestsAsync",
	    //    progressBarModel,
	    //    _commonComponentRenderers,
	    //    _dispatcher,
	    //    TimeSpan.FromMilliseconds(-1));
	    
	    NotificationHelper.DispatchInformative(
	        "DiscoverTestsAsync",
	        "DiscoverTestsAsync",
	        _commonComponentRenderers,
	        _dispatcher,
	        TimeSpan.FromSeconds(5));

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
			    		//progressBarModel.SetProgress(
			    		//	projectsHandled * completionPercentPerProject,
			    		//	$"{projectsHandled + 1}/{localTestExplorerState.ProjectTestModelList.Count}: {treeViewProjectTestModel.Item.AbsolutePath.NameWithExtension}");
			    		projectsHandled++;
		            }
		            
		            // The previous foreach loop is hackily causing the 'discover' terminal command to be enqueued
		            // to the 'executionTerminal'.
		            //
		            // The issue is, we need to know when the terminal command finishes.
		            // There are ways to do this, but we are using the an awkward piece of state
		            // that we shouldn't be touching (the treeview).
		            //
		            // If we enqueue a new terminal command, which does nothing in terminal, but has a continue with func
		            // we can time the code to run once all the tests have been discovered.
		            //
		            // TODO: This solution is incredibly hacky and nonsensical. It needs to be reworked.
		            
		            var terminalCommand = new TerminalCommand(
					    Key<TerminalCommand>.NewKey(),
					    new FormattedCommand(string.Empty, Array.Empty<string>()),
					    ContinueWith: Task_SumEachProjectTestCount);
		            
		            var executionTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_TERMINAL_KEY];
		            executionTerminal.EnqueueCommand(terminalCommand);
		        }
		    	
		    	//progressBarModel.SetProgress(1, $"Finished discovering tests in: {dotNetSolutionModel.AbsolutePath.NameWithExtension}", string.Empty);
    		}
    		catch (Exception e)
			{
				//var currentProgress = progressBarModel.GetProgress();
				//progressBarModel.SetProgress(currentProgress, e.ToString());
				
				NotificationHelper.DispatchError(
			        "DiscoverTestsAsync",
			        e.ToString(),
			        _commonComponentRenderers,
			        _dispatcher,
			        TimeSpan.FromSeconds(5));
			}
			finally
			{
				//progressBarModel.Dispose();
			}
    	});
    	
    	return Task.CompletedTask;
    }
    
    public Task Task_SumEachProjectTestCount()
    {
    	var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

        if (dotNetSolutionModel is null)
            return Task.CompletedTask;
    
    	var totalTestCount = 0;
    	var notRanTestHashSet = ImmutableHashSet<string>.Empty;
    	
    	var containsTestsTreeViewGroup = new TreeViewGroup(
	    	"Found",
            true,
            true);
            
        var noTestsTreeViewGroup = new TreeViewGroup(
	    	"Empty",
            true,
            true);
    
    	if (_treeViewService.TryGetTreeViewContainer(TestExplorerState.TreeViewTestExplorerKey, out var treeViewContainer))
        {
        	if (treeViewContainer.RootNode is not TreeViewAdhoc treeViewAdhoc)
        		return Task.CompletedTask;
        		
            foreach (var treeViewProject in treeViewAdhoc.ChildList)
            {
            	if (treeViewProject is not TreeViewProjectTestModel treeViewProjectTestModel)
            		return Task.CompletedTask;
            
            	totalTestCount += treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput?.Count ?? 0;
            	
            	if ((treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput?.Count ?? 0) > 0)
            		containsTestsTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
            	else
            		noTestsTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
            	
            	if (treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput is not null)
            	{
            		foreach (var output in treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput)
	            	{
	            		notRanTestHashSet = notRanTestHashSet.Add(output);
	            	}
            	}
            }
            
            containsTestsTreeViewGroup.LinkChildren(new(), containsTestsTreeViewGroup.ChildList);
            noTestsTreeViewGroup.LinkChildren(new(), noTestsTreeViewGroup.ChildList);
            
            var nextTreeViewAdhoc = TreeViewAdhoc.ConstructTreeViewAdhoc(
            	containsTestsTreeViewGroup,
            	noTestsTreeViewGroup);
            	
            nextTreeViewAdhoc.LinkChildren(new(), nextTreeViewAdhoc.ChildList);
            
            _treeViewService.SetRoot(TestExplorerState.TreeViewTestExplorerKey, nextTreeViewAdhoc);
        }
    
    	_dispatcher.Dispatch(new TestExplorerState.WithAction(inState => inState with
        {
            TotalTestCount = totalTestCount,
            NotRanTestHashSet = notRanTestHashSet,
            SolutionFilePath = dotNetSolutionModel.AbsolutePath.Value
        }));
        
        return Task.CompletedTask;
    }
}
