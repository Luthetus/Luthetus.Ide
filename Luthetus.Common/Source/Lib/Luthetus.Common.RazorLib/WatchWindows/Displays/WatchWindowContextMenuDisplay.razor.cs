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
    public TreeViewCommandParameter TreeViewCommandParameter { get; set; } = null!;

    public static string GetContextMenuCssStyleString(TreeViewCommandParameter? commandParameter)
    {
        if (commandParameter?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left = $"left: {commandParameter.ContextMenuFixedPosition.LeftPositionInPixels}px;";
        var top = $"top: {commandParameter.ContextMenuFixedPosition.TopPositionInPixels}px;";

        return $"{left} {top} position: fixed;";
    }

    private MenuRecord GetMenuRecord(TreeViewCommandParameter treeViewCommandParameter)
    {
        var menuOptionRecordBag = new List<MenuOptionRecord>();

        menuOptionRecordBag.Add(
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
                            if (treeViewCommandParameter.TargetNode is null)
                                return;

                            await treeViewCommandParameter.TargetNode.LoadChildBagAsync();

                            TreeViewService.ReRenderNode(
                                WatchWindowDisplay.TreeViewStateKey,
                                treeViewCommandParameter.TargetNode);

                            await InvokeAsync(StateHasChanged);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }, CancellationToken.None);
                }));

        return new MenuRecord(menuOptionRecordBag.ToImmutableArray());
    }
}