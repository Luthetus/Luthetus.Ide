using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays;

public partial class CompilerServiceExplorerTreeViewContextMenu : ComponentBase
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewCommandParameter TreeViewCommandParameter { get; set; } = null!;

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

    private MenuRecord GetMenuRecord(TreeViewCommandParameter treeViewCommandParameter)
    {
        return MenuRecord.Empty;
    }

    public static string GetContextMenuCssStyleString(
        TreeViewCommandParameter? treeViewCommandParameter)
    {
        if (treeViewCommandParameter?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left =
            $"left: {treeViewCommandParameter.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";

        var top =
            $"top: {treeViewCommandParameter.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
}