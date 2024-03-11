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
    public TreeViewCommandArgs TreeViewCommandArgs { get; set; } = null!;

    public static string GetContextMenuCssStyleString(TreeViewCommandArgs? commandArgs)
    {
        if (commandArgs?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left = $"left: {commandArgs.ContextMenuFixedPosition.LeftPositionInPixels}px;";
        var top = $"top: {commandArgs.ContextMenuFixedPosition.TopPositionInPixels}px;";

        return $"{left} {top} position: fixed;";
    }

    private MenuRecord GetMenuRecord(TreeViewCommandArgs treeViewCommandArgs)
    {
        var menuOptionRecordList = new List<MenuOptionRecord>();

        menuOptionRecordList.Add(
            new MenuOptionRecord(
                "Refresh",
                MenuOptionKind.Other,
                OnClick: () =>
                {
                    // ICommonBackgroundTaskQueue does not work well here because
                    // this Task does not need to be tracked.
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            if (treeViewCommandArgs.NodeThatReceivedMouseEvent is null)
                                return;

                            await treeViewCommandArgs.NodeThatReceivedMouseEvent.LoadChildListAsync();

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
                }));

        return new MenuRecord(menuOptionRecordList.ToImmutableArray());
    }
}