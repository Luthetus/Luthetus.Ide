using System.Collections.Immutable;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.States;

public partial class TestExplorerScheduler
{
    public async ValueTask Task_ConstructTreeView()
    {
        var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

        if (dotNetSolutionModel is null)
            return;

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
                return;
            
            var projectFileText = await _fileSystemProvider.File.ReadAllTextAsync(treeViewProjectTestModel.Item.AbsolutePath.Value);
            
            if (projectFileText.Contains("xunit"))
            {
            	if (!NoTestsTreeViewGroup.ChildList.Any(x =>
            		((TreeViewProjectTestModel)x).Item.AbsolutePath.Value == treeViewProjectTestModel.Item.AbsolutePath.Value))
            	{
            		NoTestsTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
            	}
            }
            else
            {
            	if (!NotValidProjectForUnitTestTreeViewGroup.ChildList.Any(x =>
            		((TreeViewProjectTestModel)x).Item.AbsolutePath.Value == treeViewProjectTestModel.Item.AbsolutePath.Value))
            	{
            		NotValidProjectForUnitTestTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
            	}
            }

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
							treeViewProjectTestModel.Item.TestNameFullyQualifiedList = _dotNetCliOutputParser.ParseOutputLineDotNetTestListTests(
		        				treeViewProjectTestModel.Item.TerminalCommandParsed.OutputCache.ToString());

							// THINKING_ABOUT_TREE_VIEW();
							{
								var splitOutputList = (treeViewProjectTestModel.Item.TestNameFullyQualifiedList ?? new())
									.Select(x =>
									{
										// Theory, InlineData
										// ------------------
										// I don't have it in me to fix this at the moment.
										// Need to handle these differently.
										//
										// Going to just take the non-argumented version
										// for now.
										//
										// It will run all the [InlineData] in one go
										// but can't single out yet. (2025-01-25)
										var openParenthesisIndex = x.IndexOf("(");
										if (openParenthesisIndex != -1)
											x = x[..openParenthesisIndex];
										
										return x.Split('.');
									});

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

		var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(new []
		{
			ContainsTestsTreeViewGroup,
        	NoTestsTreeViewGroup,
        	ThrewAnExceptionTreeViewGroup,
        	NotValidProjectForUnitTestTreeViewGroup
		});
		
		adhocRoot.LinkChildren(new(), adhocRoot.ChildList);
		
		ContainsTestsTreeViewGroup.LinkChildren(new(), ContainsTestsTreeViewGroup.ChildList);
    	NoTestsTreeViewGroup.LinkChildren(new(), NoTestsTreeViewGroup.ChildList);
    	ThrewAnExceptionTreeViewGroup.LinkChildren(new(), ThrewAnExceptionTreeViewGroup.ChildList);
    	NotValidProjectForUnitTestTreeViewGroup.LinkChildren(new(), NotValidProjectForUnitTestTreeViewGroup.ChildList);
        
        var firstNode = localTreeViewProjectTestModelList.FirstOrDefault();

        var activeNodes = new List<TreeViewNoType> { ContainsTestsTreeViewGroup };

        if (!_treeViewService.TryGetTreeViewContainer(TestExplorerState.TreeViewTestExplorerKey, out _))
        {
            _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                TestExplorerState.TreeViewTestExplorerKey,
                adhocRoot,
                activeNodes));
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
	    	var cancellationTokenSource = new CancellationTokenSource();
	    	var cancellationToken = cancellationTokenSource.Token;
	    	
	    	var progressBarModel = new ProgressBarModel(0, "parsing...")
	    	{
	    		OnCancelFunc = () =>
	    		{
	    			cancellationTokenSource.Cancel();
	    			cancellationTokenSource.Dispose();
	    			return Task.CompletedTask;
	    		}
	    	};

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
			
				var completionPercentPerProject = 1.0 / (double)NoTestsTreeViewGroup.ChildList.Count;
	    		var projectsHandled = 0;
	    		
	    		if (_treeViewService.TryGetTreeViewContainer(TestExplorerState.TreeViewTestExplorerKey, out var treeViewContainer))
		        {
		        	if (treeViewContainer.RootNode is not TreeViewAdhoc treeViewAdhoc)
						return;
						
					var dotNetProjectListLength = NoTestsTreeViewGroup.ChildList.Count;
		        		
		            foreach (var treeViewProject in NoTestsTreeViewGroup.ChildList)
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
		            
		            	cancellationToken.ThrowIfCancellationRequested();
		            
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
				if (e is OperationCanceledException)
					progressBarModel.IsCancelled = true;
			
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
    	
    	Console.WriteLine($"NoTestsTreeViewGroup.ChildList.Count: {NoTestsTreeViewGroup.ChildList.Count}");
    	if (_treeViewService.TryGetTreeViewContainer(TestExplorerState.TreeViewTestExplorerKey, out var treeViewContainer))
        {
        	if (treeViewContainer.RootNode is not TreeViewAdhoc treeViewAdhoc)
        		return Task.CompletedTask;
        		
            foreach (var treeViewProject in NoTestsTreeViewGroup.ChildList)
            {
            	if (treeViewProject is not TreeViewProjectTestModel treeViewProjectTestModel)
            		return Task.CompletedTask;
            
            	totalTestCount += treeViewProjectTestModel.Item.TestNameFullyQualifiedList?.Count ?? 0;
            	
            	MoveNodeToCorrectBranch(treeViewProjectTestModel);
            	
            	/*if (treeViewProjectTestModel.Item.TestNameFullyQualifiedList is not null)
            	{
            		if (treeViewProjectTestModel.Item.TestNameFullyQualifiedList.Count > 0)
            		{
            			ContainsTestsTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
            		}
            		else
            		{
            			NoTestsTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
            		}
            	}
            	else
            	{
            		if (treeViewProjectTestModel.Item.TerminalCommandParsed is not null &&
            			treeViewProjectTestModel.Item.TerminalCommandParsed.OutputCache.ToString().Contains("threw an exception"))
            		{
            			ThrewAnExceptionTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
            		}
            		else
            		{
            			NotValidProjectForUnitTestTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
            		}
            	}*/
            	
            	if (treeViewProjectTestModel.Item.TestNameFullyQualifiedList is not null)
            	{
            		foreach (var output in treeViewProjectTestModel.Item.TestNameFullyQualifiedList)
	            	{
	            		notRanTestHashSet = notRanTestHashSet.Add(output);
	            	}
            	}
            }
            
            ContainsTestsTreeViewGroup.LinkChildren(new(), ContainsTestsTreeViewGroup.ChildList);
            NoTestsTreeViewGroup.LinkChildren(new(), NoTestsTreeViewGroup.ChildList);
            ThrewAnExceptionTreeViewGroup.LinkChildren(new(), ThrewAnExceptionTreeViewGroup.ChildList);
            NotValidProjectForUnitTestTreeViewGroup.LinkChildren(new(), NotValidProjectForUnitTestTreeViewGroup.ChildList);
            
            var nextTreeViewAdhoc = TreeViewAdhoc.ConstructTreeViewAdhoc(
            	ContainsTestsTreeViewGroup,
            	NoTestsTreeViewGroup,
            	ThrewAnExceptionTreeViewGroup,
            	NotValidProjectForUnitTestTreeViewGroup);
            	
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
    
    /// <summary>
	/// This is strategic spaghetti code that is part of my master plan.
	/// Don't @ me on teams; I won't respond.
	/// </summary>
	public void MoveNodeToCorrectBranch(TreeViewProjectTestModel treeViewProjectTestModel)
	{
		if (treeViewProjectTestModel.Parent is null)
			return;
		
		if (!_treeViewService.TryGetTreeViewContainer(TestExplorerState.TreeViewTestExplorerKey, out var treeViewContainer))
			return;
		
		// containsTestsTreeViewGroup
		var containsTestsTreeViewGroupList = treeViewContainer.RootNode.ChildList
			.Where(x =>
				x is TreeViewGroup tvg &&
				tvg.Item == "Have tests");
		if (containsTestsTreeViewGroupList.Count() != 1)
			return;
		var containsTestsTreeViewGroup = containsTestsTreeViewGroupList.Single();
		
		// noTestsTreeViewGroup
		var noTestsTreeViewGroupList = treeViewContainer.RootNode.ChildList
			.Where(x =>
				x is TreeViewGroup tvg &&
				tvg.Item == "Have tests");
		if (noTestsTreeViewGroupList.Count() != 1)
			return;
		var noTestsTreeViewGroup = noTestsTreeViewGroupList.Single();

		// threwAnExceptionTreeViewGroup
		var threwAnExceptionTreeViewGroupList = treeViewContainer.RootNode.ChildList
			.Where(x =>
				x is TreeViewGroup tvg &&
				tvg.Item == "Projects that threw an exception during discovery");
		if (threwAnExceptionTreeViewGroupList.Count() != 1)
			return;
		var threwAnExceptionTreeViewGroup = threwAnExceptionTreeViewGroupList.Single();		
		
		// notValidProjectForUnitTestTreeViewGroup
		var notValidProjectForUnitTestTreeViewGroupList = treeViewContainer.RootNode.ChildList
			.Where(x =>
				x is TreeViewGroup tvg &&
				tvg.Item == "Not a test-project");
		if (notValidProjectForUnitTestTreeViewGroupList.Count() != 1)
			return;
		var notValidProjectForUnitTestTreeViewGroup = notValidProjectForUnitTestTreeViewGroupList.Single();
			
		treeViewProjectTestModel.Parent.ChildList.Remove(treeViewProjectTestModel);
		treeViewProjectTestModel.Parent.LinkChildren(
			treeViewProjectTestModel.Parent.ChildList,
			treeViewProjectTestModel.Parent.ChildList);
		_treeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, treeViewProjectTestModel.Parent);
				
		if (treeViewProjectTestModel.Item.TestNameFullyQualifiedList is not null)
    	{
    		if (treeViewProjectTestModel.Item.TestNameFullyQualifiedList.Count > 0)
    		{
    			containsTestsTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
    			containsTestsTreeViewGroup.LinkChildren(
					containsTestsTreeViewGroup.ChildList,
					containsTestsTreeViewGroup.ChildList);
				_treeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, containsTestsTreeViewGroup);
    		}
    		else
    		{
    			noTestsTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
    			noTestsTreeViewGroup.LinkChildren(
					noTestsTreeViewGroup.ChildList,
					noTestsTreeViewGroup.ChildList);
				_treeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, noTestsTreeViewGroup);
    		}
    	}
    	else
    	{
    		if (treeViewProjectTestModel.Item.TerminalCommandParsed is not null &&
    			treeViewProjectTestModel.Item.TerminalCommandParsed.OutputCache.ToString().Contains("threw an exception"))
    		{
    			threwAnExceptionTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
    			threwAnExceptionTreeViewGroup.LinkChildren(
					threwAnExceptionTreeViewGroup.ChildList,
					threwAnExceptionTreeViewGroup.ChildList);
				_treeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, threwAnExceptionTreeViewGroup);
    		}
    		else
    		{
    			notValidProjectForUnitTestTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
    			notValidProjectForUnitTestTreeViewGroup.LinkChildren(
					notValidProjectForUnitTestTreeViewGroup.ChildList,
					notValidProjectForUnitTestTreeViewGroup.ChildList);
				_treeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, notValidProjectForUnitTestTreeViewGroup);
    		}
    	}
	}
}
