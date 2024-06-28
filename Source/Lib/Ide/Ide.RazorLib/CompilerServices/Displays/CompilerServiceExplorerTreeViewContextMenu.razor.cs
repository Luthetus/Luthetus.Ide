using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays;

public partial class CompilerServiceExplorerTreeViewContextMenu : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewCommandArgs TreeViewCommandArgs { get; set; } = null!;

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

	private (TreeViewCommandArgs treeViewCommandArgs, MenuRecord menuRecord) _previousGetMenuRecordInvocation;

    private MenuRecord GetMenuRecord(TreeViewCommandArgs treeViewCommandArgs)
    {
		if (_previousGetMenuRecordInvocation.treeViewCommandArgs == treeViewCommandArgs)
			return _previousGetMenuRecordInvocation.menuRecord;

		var menuOptionList = new List<MenuOptionRecord>();

		menuOptionList.Add(new(
			"Rewrite ddropdown",
			MenuOptionKind.Other));

		var menuRecord = new MenuRecord(menuOptionList.ToImmutableArray());

		_previousGetMenuRecordInvocation = (treeViewCommandArgs, menuRecord);

        return menuRecord;;
    }

    public static string GetContextMenuCssStyleString(TreeViewCommandArgs? treeViewCommandArgs)
    {
        if (treeViewCommandArgs?.ContextMenuFixedPosition is null)
            return "display: none;";

        var left =
            $"left: {treeViewCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels.ToCssValue()}px;";

        var top =
            $"top: {treeViewCommandArgs.ContextMenuFixedPosition.TopPositionInPixels.ToCssValue()}px;";

        return $"{left} {top}";
    }
}