using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Common.RazorLib.Tabs.Displays;

public partial class TabListDisplay : ComponentBase
{
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
	private IDropdownService DropdownService { get; set; } = null!;

	/// <summary>
	/// The list provided should not be modified after passing it as a parameter..
	/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
	/// </summary>
	[Parameter, EditorRequired]
	public List<ITab> TabList { get; set; } = null!;
	
	[Parameter]
	public string CssClassString { get; set; } = string.Empty;
	
    public async Task NotifyStateChangedAsync()
	{
		await InvokeAsync(StateHasChanged);
	}

	private Task HandleTabButtonOnContextMenu(TabContextMenuEventArgs tabContextMenuEventArgs)
    {
		var dropdownRecord = new DropdownRecord(
			TabContextMenu.ContextMenuEventDropdownKey,
			tabContextMenuEventArgs.MouseEventArgs.ClientX,
			tabContextMenuEventArgs.MouseEventArgs.ClientY,
			typeof(TabContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(TabContextMenu.TabContextMenuEventArgs),
					tabContextMenuEventArgs
				}
			},
			restoreFocusOnClose: null);

        DropdownService.ReduceRegisterAction(dropdownRecord);
        return Task.CompletedTask;
    }
}