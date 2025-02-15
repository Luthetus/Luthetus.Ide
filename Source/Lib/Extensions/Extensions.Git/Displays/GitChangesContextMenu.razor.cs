using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.Git.Models;

namespace Luthetus.Extensions.Git.Displays;

public partial class GitChangesContextMenu : ComponentBase
{
	[CascadingParameter]
    public GitState GitState { get; set; } = null!;
    [CascadingParameter]
    public DropdownRecord? Dropdown { get; set; }

	[Parameter, EditorRequired]
    public TreeViewCommandArgs TreeViewCommandArgs { get; set; }

    public static readonly Key<DropdownRecord> ContextMenuEventDropdownKey = Key<DropdownRecord>.NewKey();

	private MenuRecord? _menuRecord = null;
	private bool _htmlElementDimensionsChanged = false;

	private (TreeViewCommandArgs treeViewCommandArgs, MenuRecord menuRecord) _previousGetMenuRecordInvocation;

    protected override async Task OnInitializedAsync()
    {
        // Usage of 'OnInitializedAsync' lifecycle method ensure the context menu is only rendered once.
		// Otherwise, one might have the context menu's options change out from under them.
        _menuRecord = await GetMenuRecord(TreeViewCommandArgs).ConfigureAwait(false);
		_htmlElementDimensionsChanged = true;
		await InvokeAsync(StateHasChanged);

        await base.OnInitializedAsync();
    }
    
    protected override void OnAfterRender(bool firstRender)
	{
		var localDropdown = Dropdown;

		if (localDropdown is not null && _htmlElementDimensionsChanged)
		{
			_htmlElementDimensionsChanged = false;
			localDropdown.OnHtmlElementDimensionsChanged();
		}
		
		base.OnAfterRender(firstRender);
	}

    private async Task<MenuRecord> GetMenuRecord(TreeViewCommandArgs commandArgs, bool isRecursiveCall = false)
    {
		if (!isRecursiveCall && _previousGetMenuRecordInvocation.treeViewCommandArgs == commandArgs)
			return _previousGetMenuRecordInvocation.menuRecord;

		if (!isRecursiveCall && commandArgs.TreeViewContainer.SelectedNodeList.Count > 1)
		{
			return await GetMultiSelectionMenuRecord(commandArgs).ConfigureAwait(false);
		}

        if (commandArgs.NodeThatReceivedMouseEvent is null)
		{
			var menuRecord = MenuRecord.GetEmpty();
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}

        var menuRecordsList = new List<MenuOptionRecord>();

        if (!menuRecordsList.Any())
		{
			var menuRecord = MenuRecord.GetEmpty();
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}

		// Default case
		{
			var menuRecord = new MenuRecord(menuRecordsList);
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return menuRecord;
		}
    }

	private Task<MenuRecord> GetMultiSelectionMenuRecord(TreeViewCommandArgs commandArgs)
	{
		var menuOptionRecordList = new List<MenuOptionRecord>();

		if (!menuOptionRecordList.Any())
		{
			var menuRecord = MenuRecord.GetEmpty();
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return Task.FromResult(menuRecord);
		}

		// Default case
		{
			var menuRecord = new MenuRecord(menuOptionRecordList);
			_previousGetMenuRecordInvocation = (commandArgs, menuRecord);
			return Task.FromResult(menuRecord);
		}
	}
}