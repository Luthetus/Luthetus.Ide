using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Common.RazorLib.Dropdown.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.Common.RazorLib.Menu.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Displays;

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