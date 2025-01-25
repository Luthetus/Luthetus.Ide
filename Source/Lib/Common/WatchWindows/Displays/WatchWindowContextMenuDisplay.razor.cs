using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

public partial class WatchWindowContextMenuDisplay : ComponentBase
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewCommandArgs TreeViewCommandArgs { get; set; }

	private (TreeViewCommandArgs treeViewCommandArgs, MenuRecord menuRecord) _previousGetMenuRecordInvocation;

    private MenuRecord GetMenuRecord(TreeViewCommandArgs treeViewCommandArgs)
    {
		if (_previousGetMenuRecordInvocation.treeViewCommandArgs == treeViewCommandArgs)
			return _previousGetMenuRecordInvocation.menuRecord;

        var menuOptionRecordList = new List<MenuOptionRecord>();

        menuOptionRecordList.Add(
            new MenuOptionRecord(
                "Refresh",
                MenuOptionKind.Other,
                OnClickFunc: () =>
                {
                    // ICommonBackgroundTaskQueue does not work well here because
                    // this Task does not need to be tracked.
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            if (treeViewCommandArgs.NodeThatReceivedMouseEvent is null)
                                return;

                            await treeViewCommandArgs.NodeThatReceivedMouseEvent
                                .LoadChildListAsync()
                                .ConfigureAwait(false);

                            TreeViewService.ReRenderNode(
                                WatchWindowDisplay.TreeViewContainerKey,
                                treeViewCommandArgs.NodeThatReceivedMouseEvent);

                            await InvokeAsync(StateHasChanged);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                   }, CancellationToken.None);

                    return Task.CompletedTask;
                }));

		// Default case
		{
			var menuRecord = new MenuRecord(menuOptionRecordList.ToImmutableArray());
			_previousGetMenuRecordInvocation = (treeViewCommandArgs, menuRecord);
			return menuRecord;
		}
    }
}