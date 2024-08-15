using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using System.Text;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.TestExplorers.States;

namespace Luthetus.Extensions.DotNet.TestExplorers.Displays.Internals;

public partial class TestExplorerContextMenu : ComponentBase
{
	[Inject]
	private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
	private ITreeViewService TreeViewService { get; set; } = null!;

	[CascadingParameter]
	public TestExplorerRenderBatchValidated RenderBatch { get; set; } = null!;

	[Parameter, EditorRequired]
	public TreeViewCommandArgs TreeViewCommandArgs { get; set; } = null!;

	public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();
	public static readonly Key<TerminalCommandRequest> DotNetTestByFullyQualifiedNameFormattedTerminalCommandRequestKey = Key<TerminalCommandRequest>.NewKey();

	// TODO: If one changes from 'OnInitializedAsync' an infinite loop is very likely to occur (2024-06-29)
	private MenuRecord? _menuRecord = null;

	protected override async Task OnInitializedAsync()
	{
		// Usage of 'OnInitializedAsync' lifecycle method ensure the context menu is only rendered once.
		// Otherwise, one might have the context menu's options change out from under them.
		_menuRecord = await GetMenuRecord(TreeViewCommandArgs).ConfigureAwait(false);
		await InvokeAsync(StateHasChanged);

		await base.OnInitializedAsync();
	}

	private async Task<MenuRecord> GetMenuRecord(TreeViewCommandArgs commandArgs, bool isRecursiveCall = false)
	{
		if (!isRecursiveCall && commandArgs.TreeViewContainer.SelectedNodeList.Count > 1)
		{
			return await GetMultiSelectionMenuRecord(commandArgs).ConfigureAwait(false);
		}

		if (commandArgs.NodeThatReceivedMouseEvent is null)
			return MenuRecord.Empty;

		var menuRecordsList = new List<MenuOptionRecord>();

		if (commandArgs.NodeThatReceivedMouseEvent is TreeViewStringFragment treeViewStringFragment)
		{
			var target = treeViewStringFragment;
			var fullyQualifiedNameBuilder = new StringBuilder(target.Item.Value);

			while (target.Parent is TreeViewStringFragment parentNode)
			{
				fullyQualifiedNameBuilder.Insert(0, $"{parentNode.Item.Value}.");
				target = parentNode;
			}

			if (target.Parent is TreeViewProjectTestModel treeViewProjectTestModel &&
				treeViewStringFragment.Item.IsEndpoint)
			{
				var fullyQualifiedName = fullyQualifiedNameBuilder.ToString();

				var menuOptionRecord = GetEndPointMenuOption(
					treeViewStringFragment,
					treeViewProjectTestModel,
					fullyQualifiedName);

				menuRecordsList.Add(menuOptionRecord);
			}
			else
			{
				menuRecordsList.AddRange(await GetNamespaceMenuOption(
					treeViewStringFragment,
					commandArgs,
					isRecursiveCall));
			}
		}
		else if (commandArgs.NodeThatReceivedMouseEvent is TreeViewProjectTestModel treeViewProjectTestModel)
		{
			menuRecordsList.Add(new MenuOptionRecord(
				$"Refresh: {treeViewProjectTestModel.Item.AbsolutePath.NameWithExtension}",
				MenuOptionKind.Other,
				OnClickFunc: async () =>
				{
					// TODO: This code is not concurrency safe with 'TestExplorerScheduler.Task_DiscoverTests()'
					Dispatcher.Dispatch(new TestExplorerState.WithAction(inState =>
					{
						if (treeViewProjectTestModel.Item.TestNameFullyQualifiedList is null)
							return inState;
					
						var mutablePassedTestHashSet = inState.PassedTestHashSet.ToHashSet();
						var mutableNotRanTestHashSet = inState.NotRanTestHashSet.ToHashSet();
						var mutableFailedTestHashSet = inState.FailedTestHashSet.ToHashSet();
						
						foreach (var fullyQualifiedTestName in treeViewProjectTestModel.Item.TestNameFullyQualifiedList)
						{
							mutablePassedTestHashSet.Remove(fullyQualifiedTestName);
							mutableNotRanTestHashSet.Remove(fullyQualifiedTestName);
							mutableFailedTestHashSet.Remove(fullyQualifiedTestName);
						}
						
						return inState with
				        {
				            PassedTestHashSet = mutablePassedTestHashSet.ToImmutableHashSet(),
				            NotRanTestHashSet = mutableNotRanTestHashSet.ToImmutableHashSet(),
				            FailedTestHashSet = mutableFailedTestHashSet.ToImmutableHashSet(),
				        };
				    }));
			        
					treeViewProjectTestModel.Item.TestNameFullyQualifiedList = null;
					TreeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, treeViewProjectTestModel);
					
					await treeViewProjectTestModel.LoadChildListAsync();
					TreeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, treeViewProjectTestModel);
					
					MoveNodeToCorrectBranch(treeViewProjectTestModel);
					
					Dispatcher.Dispatch(new TestExplorerState.WithAction(inState =>
					{
						if (treeViewProjectTestModel.Item.TestNameFullyQualifiedList is null)
							return inState;
					
						var mutableNotRanTestHashSet = inState.NotRanTestHashSet.ToHashSet();
						
						foreach (var fullyQualifiedTestName in treeViewProjectTestModel.Item.TestNameFullyQualifiedList)
						{
							mutableNotRanTestHashSet.Add(fullyQualifiedTestName);
						}
						
						return inState with
				        {
				            NotRanTestHashSet = mutableNotRanTestHashSet.ToImmutableHashSet(),
				        };
				    }));
				}));
		}

		if (!menuRecordsList.Any())
			return MenuRecord.Empty;

		return new MenuRecord(menuRecordsList.ToImmutableArray());
	}

	private MenuOptionRecord GetEndPointMenuOption(
		TreeViewStringFragment treeViewStringFragment,
		TreeViewProjectTestModel treeViewProjectTestModel,
		string fullyQualifiedName)
	{
		var menuOptionRecord = new MenuOptionRecord(
			$"Run: {treeViewStringFragment.Item.Value}",
			MenuOptionKind.Other,
			OnClickFunc: () =>
			{
				if (treeViewProjectTestModel.Item.AbsolutePath.ParentDirectory is not null)
				{
					BackgroundTaskService.Enqueue(
						Key<IBackgroundTask>.NewKey(),
						BlockingBackgroundTaskWorker.GetQueueKey(),
						"RunTestByFullyQualifiedName",
						() =>
						{
							RunTestByFullyQualifiedName(
								treeViewStringFragment,
								fullyQualifiedName,
								treeViewProjectTestModel.Item.AbsolutePath.ParentDirectory.Value);

							return Task.CompletedTask;
						});
				}

				return Task.CompletedTask;
			});

		return menuOptionRecord;
	}

	private async Task<List<MenuOptionRecord>> GetNamespaceMenuOption(
		TreeViewStringFragment treeViewStringFragment,
		TreeViewCommandArgs commandArgs,
		bool isRecursiveCall = false)
	{
		void RecursiveStep(TreeViewStringFragment treeViewStringFragmentNamespace, List<TreeViewNoType> fabricateSelectedNodeList)
		{
			foreach (var childNode in treeViewStringFragmentNamespace.ChildList)
			{
				if (childNode is TreeViewStringFragment childTreeViewStringFragment)
				{
					if (childTreeViewStringFragment.Item.IsEndpoint)
					{
						fabricateSelectedNodeList.Add(childTreeViewStringFragment);
					}
					else
					{
						RecursiveStep(childTreeViewStringFragment, fabricateSelectedNodeList);
					}
				}
			}
		}

		var fabricateSelectedNodeList = new List<TreeViewNoType>();

		RecursiveStep(treeViewStringFragment, fabricateSelectedNodeList);

		var fabricateTreeViewContainer = commandArgs.TreeViewContainer with
		{
			SelectedNodeList = fabricateSelectedNodeList.ToImmutableList()
		};

		var fabricateCommandArgs = new TreeViewCommandArgs(
			commandArgs.TreeViewService,
			fabricateTreeViewContainer,
			commandArgs.NodeThatReceivedMouseEvent,
			commandArgs.RestoreFocusToTreeView,
			commandArgs.ContextMenuFixedPosition,
			commandArgs.MouseEventArgs,
			commandArgs.KeyboardEventArgs);

		var multiSelectionMenuRecord = await GetMultiSelectionMenuRecord(fabricateCommandArgs);

		var menuOptionRecord = new MenuOptionRecord(
			$"Namespace: {treeViewStringFragment.Item.Value} | {fabricateSelectedNodeList.Count}",
			MenuOptionKind.Other,
			SubMenu: multiSelectionMenuRecord);

		return new() { menuOptionRecord };
	}

	private async Task<MenuRecord> GetMultiSelectionMenuRecord(TreeViewCommandArgs commandArgs)
	{
		var menuOptionRecordList = new List<MenuOptionRecord>();
		Func<Task> runAllOnClicksWithinSelection = () => Task.CompletedTask;
		bool runAllOnClicksWithinSelectionHasEffect = false;

		foreach (var node in commandArgs.TreeViewContainer.SelectedNodeList)
		{
			MenuOptionRecord menuOption;

			if (node is TreeViewStringFragment treeViewStringFragment)
			{
				var innerTreeViewCommandArgs = new TreeViewCommandArgs(
					commandArgs.TreeViewService,
					commandArgs.TreeViewContainer,
					node,
					commandArgs.RestoreFocusToTreeView,
					commandArgs.ContextMenuFixedPosition,
					commandArgs.MouseEventArgs,
					commandArgs.KeyboardEventArgs);

				menuOption = new(
					treeViewStringFragment.Item.Value,
					MenuOptionKind.Other,
					SubMenu: await GetMenuRecord(innerTreeViewCommandArgs, true).ConfigureAwait(false));

				var copyRunAllOnClicksWithinSelection = runAllOnClicksWithinSelection;

				runAllOnClicksWithinSelection = async () =>
				{
					await copyRunAllOnClicksWithinSelection.Invoke().ConfigureAwait(false);

					if (menuOption.SubMenu?.MenuOptionList.Single().OnClickFunc is not null)
					{
						await menuOption.SubMenu.MenuOptionList
							.Single().OnClickFunc!
							.Invoke()
							.ConfigureAwait(false);
					}
				};

				runAllOnClicksWithinSelectionHasEffect = true;
			}
			else
			{
				menuOption = new(
					node.GetType().Name,
					MenuOptionKind.Other,
					SubMenu: MenuRecord.Empty);
			}

			menuOptionRecordList.Add(menuOption);
		}

		if (runAllOnClicksWithinSelectionHasEffect)
		{
			menuOptionRecordList.Insert(0, new(
				"Run all OnClicks within selection",
				MenuOptionKind.Create,
				OnClickFunc: runAllOnClicksWithinSelection));
		}

		if (!menuOptionRecordList.Any())
			return MenuRecord.Empty;

		return new MenuRecord(menuOptionRecordList.ToImmutableArray());
	}

	private void RunTestByFullyQualifiedName(
		TreeViewStringFragment treeViewStringFragment,
		string fullyQualifiedName,
		string? directoryNameForTestDiscovery)
	{
		var dotNetTestByFullyQualifiedNameFormattedCommand = DotNetCliCommandFormatter
			.FormatDotNetTestByFullyQualifiedName(fullyQualifiedName);

		if (string.IsNullOrWhiteSpace(directoryNameForTestDiscovery) ||
			string.IsNullOrWhiteSpace(fullyQualifiedName))
		{
			return;
		}

		var terminalCommandRequest = new TerminalCommandRequest(
        	dotNetTestByFullyQualifiedNameFormattedCommand.Value,
        	directoryNameForTestDiscovery,
        	treeViewStringFragment.Item.DotNetTestByFullyQualifiedNameFormattedTerminalCommandRequestKey)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		treeViewStringFragment.Item.TerminalCommandParsed = parsedCommand;
        		TreeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, treeViewStringFragment);
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		treeViewStringFragment.Item.TerminalCommandParsed = parsedCommand;
				var output = treeViewStringFragment.Item.TerminalCommandParsed?.OutputCache.ToString() ?? null;
				
				if (output is not null && output.Contains("Duration:"))
				{
					if (output.Contains("Passed!"))
					{
						Dispatcher.Dispatch(new TestExplorerState.WithAction(inState => inState with
				        {
				            PassedTestHashSet = inState.PassedTestHashSet.Add(fullyQualifiedName),
				            NotRanTestHashSet = inState.NotRanTestHashSet.Remove(fullyQualifiedName),
				            FailedTestHashSet = inState.FailedTestHashSet.Remove(fullyQualifiedName),
				        }));
					}
					else
					{
						Dispatcher.Dispatch(new TestExplorerState.WithAction(inState => inState with
				        {
				            FailedTestHashSet = inState.FailedTestHashSet.Add(fullyQualifiedName),
				            NotRanTestHashSet = inState.NotRanTestHashSet.Remove(fullyQualifiedName),
				            PassedTestHashSet = inState.PassedTestHashSet.Remove(fullyQualifiedName),
				        }));
					}
				}
			
				TreeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, treeViewStringFragment);
				return Task.CompletedTask;
        	}
        };
        
		treeViewStringFragment.Item.TerminalCommandRequest = terminalCommandRequest;
        TerminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_KEY].EnqueueCommand(terminalCommandRequest);
	}
	
	/// <summary>
	/// This is strategic spaghetti code that is part of my master plan.
	/// Don't @ me on teams; I won't respond.
	/// </summary>
	private void MoveNodeToCorrectBranch(TreeViewProjectTestModel treeViewProjectTestModel)
	{
		if (treeViewProjectTestModel.Parent is null)
			return;
		
		if (!TreeViewService.TryGetTreeViewContainer(TestExplorerState.TreeViewTestExplorerKey, out var treeViewContainer))
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
		TreeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, treeViewProjectTestModel.Parent);
				
		if (treeViewProjectTestModel.Item.TestNameFullyQualifiedList is not null)
    	{
    		if (treeViewProjectTestModel.Item.TestNameFullyQualifiedList.Count > 0)
    		{
    			containsTestsTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
    			containsTestsTreeViewGroup.LinkChildren(
					containsTestsTreeViewGroup.ChildList,
					containsTestsTreeViewGroup.ChildList);
				TreeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, containsTestsTreeViewGroup);
    		}
    		else
    		{
    			noTestsTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
    			noTestsTreeViewGroup.LinkChildren(
					noTestsTreeViewGroup.ChildList,
					noTestsTreeViewGroup.ChildList);
				TreeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, noTestsTreeViewGroup);
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
				TreeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, threwAnExceptionTreeViewGroup);
    		}
    		else
    		{
    			notValidProjectForUnitTestTreeViewGroup.ChildList.Add(treeViewProjectTestModel);
    			notValidProjectForUnitTestTreeViewGroup.LinkChildren(
					notValidProjectForUnitTestTreeViewGroup.ChildList,
					notValidProjectForUnitTestTreeViewGroup.ChildList);
				TreeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, notValidProjectForUnitTestTreeViewGroup);
    		}
    	}
	}
}