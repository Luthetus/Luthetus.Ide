using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.TestExplorers.States.TestExplorerState;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial class TestExplorerSync
{
	// When the user opens a .NET solution.
	// A C# object is made to store that chosen solution.
	private Task DotNetSolutionStateWrap_StateChangedAsync()
    {
        var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
		var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

		if (dotNetSolutionModel is null)
            return Task.CompletedTask;

		// Get the projects
        var localDotNetProjectList = dotNetSolutionModel.DotNetProjectList
			.Where(x => x.DotNetProjectKind == DotNetProjectKind.CSharpProject);

		var localProjectTestModelList = localDotNetProjectList.Select(x => new ProjectTestModel(
			x.ProjectIdGuid,
			x.AbsolutePath,
			callback => Task.CompletedTask,
			node => _treeViewService.ReRenderNode(TreeViewTestExplorerKey, node)));

		var localFormattedCommand = DotNetCliCommandFormatter.FormatDotNetTestListTests();

		// FormatDotNetTestListTests
		//
		// The dotnet cli command to discover unit tests appears to be ran
		// at some point in this method.

		var localTreeViewProjectTestModelList = localProjectTestModelList.Select(x =>
				(TreeViewNoType)new TreeViewProjectTestModel(
					x,
					_commonComponentRenderers,
					true,
					false))
			.ToArray();

		foreach (var entry in localTreeViewProjectTestModelList)
		{ // Foreach project in the solution, the discover unit tests cli command is being ran.
			var treeViewProjectTestModel = (TreeViewProjectTestModel)entry;
			
			if (string.IsNullOrWhiteSpace(treeViewProjectTestModel.Item.DirectoryNameForTestDiscovery))
                return Task.CompletedTask;

            treeViewProjectTestModel.Item.EnqueueDiscoverTestsFunc = async callback =>
			{
				var executionTerminalSession = _terminalSessionStateWrap.Value.TerminalSessionMap[
	            	TerminalSessionFacts.EXECUTION_TERMINAL_SESSION_KEY];
	
		        var dotNetTestListTestsCommand = new TerminalCommand(
		            treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey,
		            localFormattedCommand,
		            treeViewProjectTestModel.Item.DirectoryNameForTestDiscovery,
		            CancellationToken.None,
		            async () => 
					{
						try
						{
							// This lambda is the 'ContinueWith' for the 'TerminalCommand'
							// datatype.
							//
							// That is to say, after a terminal command finishes,
							// the 'ContinueWith' Func is invoked.
							//
							// I can use the 'Code Search' dialog to find the 'TerminalCommand'
							// source code.
							// { Ctrl + , } is the keybind OR use the header 'Tools' dropdown.
							var output = executionTerminalSession.ReadStandardOut(treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey);

							if (output is null)
								return;

	// I'm going to re-run the Find All so I can confirm that
	// I made all the renames.

							// The width of this line seems to be bugged out.
							// I put a newline so we can see the entire line.
							//
							// The DotNetCliOutputLexer is being used here to parse out the
							// Discovered unit tests.
							treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput =
								DotNetCliOutputParser.ParseDotNetTestListTestsTerminalOutput(output);
			
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
								await callback.Invoke(rootMap);
							}
						}
						catch (Exception)
						{
							await callback.Invoke(new());
							throw;
						}
					},
                    () => {
                        executionTerminalSession.ClearStandardOut(
							treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey);

                        return Task.CompletedTask;
                    });

		        await executionTerminalSession.EnqueueCommandAsync(dotNetTestListTestsCommand);
			};				
		}

		var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(localTreeViewProjectTestModelList);
		var firstNode = localTreeViewProjectTestModelList.FirstOrDefault();

		var activeNodes = firstNode is null
			? Array.Empty<TreeViewNoType>()
			: new []{ firstNode };
			
		if (!_treeViewService.TryGetTreeViewContainer(TreeViewTestExplorerKey, out _))
        {
            _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                TreeViewTestExplorerKey,
                adhocRoot,
                activeNodes.ToImmutableList()));
        }
        else
        {
            _treeViewService.SetRoot(TreeViewTestExplorerKey, adhocRoot);
            
			_treeViewService.SetActiveNode(
				TreeViewTestExplorerKey,
				firstNode,
				true,
				false);
        }

		_dispatcher.Dispatch(new WithAction(inState => inState with
		{
			ProjectTestModelList = localProjectTestModelList.ToImmutableList()
		}));

        return Task.CompletedTask;
    }
}