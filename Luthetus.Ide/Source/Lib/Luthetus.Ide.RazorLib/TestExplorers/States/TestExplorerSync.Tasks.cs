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
	private Task DotNetSolutionStateWrap_StateChangedAsync()
    {
        var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
		var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

		if (dotNetSolutionModel is null)
            return Task.CompletedTask;

        var localDotNetProjectBag = dotNetSolutionModel.DotNetProjectBag
			.Where(x => x.DotNetProjectKind == DotNetProjectKind.CSharpProject);

		var localProjectTestModelBag = localDotNetProjectBag.Select(x => new ProjectTestModel(
			x.ProjectIdGuid,
			x.AbsolutePath,
			callback => Task.CompletedTask,
			node => _treeViewService.ReRenderNode(TreeViewTestExplorerKey, node)));

		var localFormattedCommand = DotNetCliCommandFormatter.FormatDotNetTestListTests();

		var localTreeViewProjectTestModelBag = localProjectTestModelBag.Select(x =>
				(TreeViewNoType)new TreeViewProjectTestModel(
					x,
					_commonComponentRenderers,
					true,
					false))
			.ToArray();

		foreach (var entry in localTreeViewProjectTestModelBag)
		{
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
							var output = executionTerminalSession.ReadStandardOut(treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey);

							if (output is null)
								return;

							treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput = DotNetCliOutputLexer.LexDotNetTestListTestsTerminalOutput(output);
			
			    			// THINKING_ABOUT_TREE_VIEW();
							{
								var splitOutputBag = treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput
									.Select(x => x.Split('.'));
						
								var rootMap = new Dictionary<string, StringFragment>();

								foreach (var splitOutput in splitOutputBag)
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

		var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(localTreeViewProjectTestModelBag);
		var firstNode = localTreeViewProjectTestModelBag.FirstOrDefault();

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
            _treeViewService.SetActiveNode(TreeViewTestExplorerKey, firstNode);
        }

		_dispatcher.Dispatch(new WithAction(inState => inState with
		{
			ProjectTestModelBag = localProjectTestModelBag.ToImmutableList()
		}));

        return Task.CompletedTask;
    }
}