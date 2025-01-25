using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Displays;

public partial class FindAllContextMenu : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewCommandArgs TreeViewCommandArgs { get; set; }

	public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

	private (TreeViewCommandArgs treeViewCommandArgs, MenuRecord menuRecord) _previousGetMenuRecordInvocation;
	
	private MenuRecord GetMenuRecord(TreeViewCommandArgs commandArgs)
    {
		if (_previousGetMenuRecordInvocation.treeViewCommandArgs == commandArgs)
			return _previousGetMenuRecordInvocation.menuRecord;

		if (commandArgs.TreeViewContainer.SelectedNodeList.Count > 1)
		{
			return GetMultiSelectionMenuRecord(commandArgs);
		}

        if (commandArgs.NodeThatReceivedMouseEvent is null)
		{
			var menuRecord = MenuRecord.Empty;
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}

        var menuRecordsList = new List<MenuOptionRecord>();

        if (!menuRecordsList.Any())
		{
			var menuRecord = MenuRecord.Empty;
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}

		// Default case
		{
			var menuRecord = new MenuRecord(menuRecordsList.ToImmutableArray());
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}
    }

	private MenuRecord GetMultiSelectionMenuRecord(TreeViewCommandArgs commandArgs)
	{
		var menuOptionRecordList = new List<MenuOptionRecord>();

		if (!menuOptionRecordList.Any())
		{
			var menuRecord = MenuRecord.Empty;
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}

		// Default case
		{
			var menuRecord = new MenuRecord(menuOptionRecordList.ToImmutableArray());
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}
	}
}