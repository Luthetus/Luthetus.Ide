using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
using Luthetus.Common.RazorLib.Reactives.Models;
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
		        	BeginWithFunc = async parsedCommand =>
		        	{
		        		treeViewProjectTestModel.Item.TerminalCommandParsed = parsedCommand;
		        	},
		        	ContinueWithFunc = async parsedCommand =>
		        	{
		        		treeViewProjectTestModel.Item.TerminalCommandParsed = parsedCommand;
		        	
						try
						{
							treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput = _dotNetCliOutputParser.ParseOutputLineDotNetTestListTests(
		        				treeViewProjectTestModel.Item.TerminalCommandParsed.OutputCache.ToString());

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
    	return _throttleDiscoverTests.RunAsync(async _ => 
    	{
	    	var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
	        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;
	
	        if (dotNetSolutionModel is null)
	            return;
	    	
	    	var localTestExplorerState = _testExplorerStateWrap.Value;
	    	
	    	var progressBarModel = new ProgressBarModel(0, "parsing...");

			NotificationHelper.DispatchProgress(
				$"Test Discovery: {dotNetSolutionModel.AbsolutePath.NameWithExtension}",
				progressBarModel,
				_commonComponentRenderers,
				_dispatcher,
				TimeSpan.FromMilliseconds(-1));
				
			var progressThrottle = new Throttle(TimeSpan.FromMilliseconds(100));
		    
			try
			{
				progressThrottle.Run(_ => 
				{
					progressBarModel.SetProgress(0, "Discovering tests...");
					return Task.CompletedTask;
				});
			
				var completionPercentPerProject = 1.0 / (double)localTestExplorerState.ProjectTestModelList.Count;
	    		var projectsHandled = 0;
	    		
	    		if (_treeViewService.TryGetTreeViewContainer(TestExplorerState.TreeViewTestExplorerKey, out var treeViewContainer))
		        {
		        	if (treeViewContainer.RootNode is not TreeViewAdhoc treeViewAdhoc)
						return;
						
					var dotNetProjectListLength = treeViewAdhoc.ChildList.Count;
		        		
		            foreach (var treeViewProject in treeViewAdhoc.ChildList)
		            {
		            	if (treeViewProject is not TreeViewProjectTestModel treeViewProjectTestModel)
		            		continue;
		            		
		            	var currentProgress = completionPercentPerProject * projectsHandled;
		            	
		            	progressThrottle.Run(_ => 
						{
							progressBarModel.SetProgress(
								currentProgress,
								$"{projectsHandled + 1}/{dotNetProjectListLength}: {treeViewProjectTestModel.Item.AbsolutePath.NameWithExtension}");
							return Task.CompletedTask;
						});
		            
		            	await treeViewProject.LoadChildListAsync();
			    		projectsHandled++;
		            }
		       
		       	 progressThrottle.Run(_ => 
					{
						progressBarModel.SetProgress(1, $"Finished test discovery: {dotNetSolutionModel.AbsolutePath.NameWithExtension}", string.Empty);
						progressBarModel.Dispose();
						return Task.CompletedTask;
					});     
		            await Task_SumEachProjectTestCount();
		        }
			}
			catch (Exception e)
			{
				var currentProgress = progressBarModel.GetProgress();
				
				progressThrottle.Run(_ => 
				{
					// TODO: Set message 2 as the error instead so we can see the project...
					//       ... that it was discovering tests for when it threw exception?
					progressBarModel.SetProgress(currentProgress, e.ToString());
					progressBarModel.Dispose();
					return Task.CompletedTask;
				});
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
