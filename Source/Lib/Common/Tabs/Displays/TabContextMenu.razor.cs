using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.Tabs.Displays;

public partial class TabContextMenu : ComponentBase
{
    [Parameter, EditorRequired]
    public TabContextMenuEventArgs TabContextMenuEventArgs { get; set; } = null!;

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

    /// <summary>
    /// The program is currently running using Photino locally on the user's computer
    /// therefore this static solution works without leaking any information.
    /// </summary>
    public static TreeViewNoType? ParentOfCutFile;

	private (TabContextMenuEventArgs tabContextMenuEventArgs, MenuRecord menuRecord) _previousGetMenuRecordInvocation;

    private MenuRecord GetMenuRecord(TabContextMenuEventArgs tabContextMenuEventArgs)
    {
		if (_previousGetMenuRecordInvocation.tabContextMenuEventArgs == tabContextMenuEventArgs)
			return _previousGetMenuRecordInvocation.menuRecord;

        var menuOptionList = new List<MenuOptionRecord>();

        menuOptionList.Add(new MenuOptionRecord(
            "Close All",
            MenuOptionKind.Delete,
            () => tabContextMenuEventArgs.Tab.TabGroup.CloseAllAsync()));

		menuOptionList.Add(new MenuOptionRecord(
            "Close Others",
            MenuOptionKind.Delete,
            () => tabContextMenuEventArgs.Tab.TabGroup.CloseOthersAsync(tabContextMenuEventArgs.Tab)));

		if (!menuOptionList.Any())
		{
			var menuRecord = MenuRecord.Empty;
			_previousGetMenuRecordInvocation = (tabContextMenuEventArgs, menuRecord);
			return menuRecord;
		}

		// Default case
		{
			var menuRecord = new MenuRecord(menuOptionList.ToImmutableArray());
			_previousGetMenuRecordInvocation = (tabContextMenuEventArgs, menuRecord);
			return menuRecord;
		}
    }
}