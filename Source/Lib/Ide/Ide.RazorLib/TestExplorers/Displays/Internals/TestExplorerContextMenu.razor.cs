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
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.Ide.RazorLib.TestExplorers.States;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays.Internals;

public partial class TestExplorerContextMenu : ComponentBase
{
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
	[Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

	[CascadingParameter]
    public TestExplorerRenderBatchValidated RenderBatch { get; set; } = null!;

	[Parameter, EditorRequired]
    public TreeViewCommandArgs TreeViewCommandArgs { get; set; } = null!;

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();
    public static readonly Key<TerminalCommand> DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey = Key<TerminalCommand>.NewKey();

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
		//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
        // =========================================================================
		FormattedCommand ThrowNotImplementedException() => throw new NotImplementedException("(2024-07-15)");
		//var dotNetTestByFullyQualifiedNameFormattedCommand = DotNetCliCommandFormatter.FormatDotNetTestByFullyQualifiedName(fullyQualifiedName);
		var dotNetTestByFullyQualifiedNameFormattedCommand = ThrowNotImplementedException();

		if (string.IsNullOrWhiteSpace(directoryNameForTestDiscovery) ||
			string.IsNullOrWhiteSpace(fullyQualifiedName))
		{
			return;
		}

		var executionTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_TERMINAL_KEY];

		/*
		//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
    	// =======================================================================
        var dotNetTestByFullyQualifiedNameTerminalCommand = new TerminalCommand(
            treeViewStringFragment.Item.DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey,
            dotNetTestByFullyQualifiedNameFormattedCommand,
			directoryNameForTestDiscovery,
			OutputParser: DotNetCliOutputParser,
			ContinueWith: () => 
			{
				BackgroundTaskService.Enqueue(
					Key<IBackgroundTask>.NewKey(),
					ContinuousBackgroundTaskWorker.GetQueueKey(),
					"CheckTestOutcome",
					() => 
					{
						var output = treeViewStringFragment.Item.TerminalCommand?.OutputBuilder?.ToString() ?? null;
						
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
						
						return Task.CompletedTask;
					});
			
				TreeViewService.ReRenderNode(TestExplorerState.TreeViewTestExplorerKey, treeViewStringFragment);
				return Task.CompletedTask;
			});

		treeViewStringFragment.Item.TerminalCommand = dotNetTestByFullyQualifiedNameTerminalCommand;

		executionTerminal.EnqueueCommand(dotNetTestByFullyQualifiedNameTerminalCommand);
		*/
	}
}