using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using System.Text;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays;

public partial class TestExplorerContextMenu : ComponentBase
{
    [Inject]
    private IState<TerminalSessionState> TerminalSessionStateWrap { get; set; } = null!;
	[Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

	[CascadingParameter]
    public TestExplorerRenderBatchValidated RenderBatch { get; set; } = null!;

	[Parameter, EditorRequired]
    public TreeViewCommandArgs TreeViewCommandArgs { get; set; } = null!;

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();
    public static readonly Key<TerminalCommand> DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey = Key<TerminalCommand>.NewKey();

    private MenuRecord GetMenuRecord(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.TargetNode is null)
            return MenuRecord.Empty;

        var menuRecordsList = new List<MenuOptionRecord>();

		if (commandArgs.TargetNode is TreeViewStringFragment treeViewStringFragment)
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

				var menuOptionRecord = new MenuOptionRecord(
					$"Run: {treeViewStringFragment.Item.Value}",
					MenuOptionKind.Other,
					OnClick: () => 
					{
						BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), BlockingBackgroundTaskWorker.GetQueueKey(),
				            "RunTestByFullyQualifiedName",
				            async () => await RunTestByFullyQualifiedName(
								treeViewStringFragment,
								fullyQualifiedName,
								treeViewProjectTestModel.Item.AbsolutePath.ParentDirectory?.Value));
					});
	
				menuRecordsList.Add(menuOptionRecord);
			}
			else
			{
				var menuOptionRecord = new MenuOptionRecord(
					$"Namespace: {treeViewStringFragment.Item.Value}",
					MenuOptionKind.Other);

				menuRecordsList.Add(menuOptionRecord);
			}
		}

        if (!menuRecordsList.Any())
            return MenuRecord.Empty;

        return new MenuRecord(menuRecordsList.ToImmutableArray());
    }

	private async Task RunTestByFullyQualifiedName(
		TreeViewStringFragment treeViewStringFragment,
		string fullyQualifiedName,
		string? directoryNameForTestDiscovery)
	{
		var dotNetTestByFullyQualifiedNameFormattedCommand = DotNetCliCommandFormatter.FormatDotNetTestByFullyQualifiedName(fullyQualifiedName);

		if (string.IsNullOrWhiteSpace(directoryNameForTestDiscovery) ||
			string.IsNullOrWhiteSpace(fullyQualifiedName))
		{
			return;
		}

		var executionTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.EXECUTION_TERMINAL_SESSION_KEY];

        var dotNetTestByFullyQualifiedNameTerminalCommand = new TerminalCommand(
            treeViewStringFragment.Item.DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey,
            dotNetTestByFullyQualifiedNameFormattedCommand,
            directoryNameForTestDiscovery,
            CancellationToken.None,
            () => Task.CompletedTask,
			() =>
			{
				executionTerminalSession.ClearStandardOut(
					treeViewStringFragment.Item.DotNetTestByFullyQualifiedNameFormattedTerminalCommandKey);

                return Task.CompletedTask;
			});

        await executionTerminalSession.EnqueueCommandAsync(dotNetTestByFullyQualifiedNameTerminalCommand);
	}

    public static string GetContextMenuCssStyleString(TreeViewCommandArgs? commandArgs)
    {
        if (commandArgs?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left =
            $"left: {commandArgs.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";

        var top =
            $"top: {commandArgs.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
}