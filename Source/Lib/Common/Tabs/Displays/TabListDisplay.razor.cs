using Fluxor;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Tabs.Displays;

public partial class TabListDisplay : ComponentBase
{
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;

	[Parameter, EditorRequired]
	public ImmutableArray<ITab> TabList { get; set; } = ImmutableArray<ITab>.Empty;
	
	[Parameter]
	public string CssClassString { get; set; } = string.Empty;

    public async Task NotifyStateChangedAsync()
	{
		await InvokeAsync(StateHasChanged);
	}

	private async Task HandleTabButtonOnContextMenu(TabContextMenuEventArgs tabContextMenuEventArgs)
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
			});

        Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
    }
}