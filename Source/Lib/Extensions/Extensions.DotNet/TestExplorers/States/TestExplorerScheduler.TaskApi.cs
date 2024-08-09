using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.States;

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
				var terminalCommandRequest = new TerminalCommandRequest(
		        	localFormattedCommand.Value,
		        	treeViewProjectTestModel.Item.DirectoryNameForTestDiscovery,
		        	treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandRequestKey)
		        {
		        	ContinueWithFunc = async parsedCommand =>
		        	{
		        		// _dotNetCliOutputParser was being used
		        	
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
		        	}
		        };

                treeViewProjectTestModel.Item.TerminalCommandRequest = terminalCommandRequest;
                
				return _terminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_KEY]
					.EnqueueCommandAsync(terminalCommandRequest);
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
		// (2024-08-06)
		// ============
		// [] Add blocking enqueue (EnqueueAsync?)
		// [] Add blocking throttle (RunAsync?)
		// [] Successfully discover the unit tests, and make the tree view.
		// [] To start, I don't fully understand what I'm about to describe here
		// 	[] It appears that on Linux the "pipe" that all my code is pushed through
		// 	   	is not as optimized as the "pipe" that is used on Windows.
		//     [] Could this be a driver issue? Do I have the wrong drivers installed on Ubuntu at the moment?
		//     [] Either way, on windows if I select text with my cursor, and drag over the text in a fast motion,
		//        	the text editor's cursor only slightly lags behind my movement (maybe 1 character of lag?).
		//     	   But, on Linux I'm seeing 10 to 20 characters of lag.
		// 	[] As well, on Windows, it seems I've hit an "optimization plateau" with respect to this
		//        	cursor text selection, and viewing the character lag.
		//     [] More specifically, this plateau appears to be in relation to me optimizing via
		//        	the UI.
		//	 [] i.e.:
		//        	Limit the amount of Blazor parameters (they are set using reflection, which is slow)
		//        	Limit the amount of Blazor components (especially when dealing with loops, because each component
		// 												  carries more overhead, than if a RenderFragment template were used.)
		//        	Limit the amount of anonymous lambdas (especially when dealing with loops,
		//												   because it causes 1 entry to re-render all entires
		//                                                   due to them sharing the same static class
		//                                                   that represents the anonymous lambda.)
		// 	[] While I do think I could benefit from more UI optimization, at this point
		//        	it likely has become an order of magnitude less optimization than "other" forms of optimization.
		// 	[] For example, I think the reason I have 1 character of lag when using my mouse to drag select text,
		//        	is because I am not throttling the amount of UI events that are coming through.
		//     [] Previously, I had used the 'batching' logic, where two successive mouse move events were deemed "redundant"
		//        	and therefore I only would take the most recent mouse event, and then discard the earlier ones.
		//     [] But, I am not sure if any batching logic even is running anymore, because I made changes to the IBackgroundTaskService,
		//        	such that there is no implicit 'await Task.Delay(...)' between background tasks.
		//     [] I like the removal of the implicit 'await Task.Delay(...)' but I would imagine this makes the likelyhood
		//        	of every mouse move event causing a background task to be very high.
		//     [] If there is a background task that needs delay, then it should itself 'await Task.Delay(...)' within
		//        	the code for its background task work item.
		// 	[] So, the on mouse move might benefit from the addition of a 'await Task.Delay(...)' within its own work item at the end
		//        	after it finishes.
    
    	return _throttleDiscoverTests.RunAsync(async _ => 
    	{
	    	var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
	        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;
	
	        if (dotNetSolutionModel is null)
	            return;
	    	
	    	var localTestExplorerState = _testExplorerStateWrap.Value;
		    
		    NotificationHelper.DispatchInformative(
		        "DiscoverTestsAsync", "DiscoverTestsAsync", _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(5));
	
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
		            		continue;
		            
		            	await treeViewProject.LoadChildListAsync();
			    		projectsHandled++;
		            }
		            
		            await Task_SumEachProjectTestCount();
		        }
			}
			catch (Exception e)
			{
				NotificationHelper.DispatchError(
			        "DiscoverTestsAsync", e.ToString(), _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(5));
			}
		});
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
