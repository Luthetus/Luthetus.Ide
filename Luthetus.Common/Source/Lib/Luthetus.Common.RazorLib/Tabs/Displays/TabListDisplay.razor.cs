using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

    private TabContextMenuEventArgs? _mostRecentTabContextMenuEventArgs;

    public async Task NotifyStateChangedAsync()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	private async Task HandleTabButtonOnContextMenu(TabContextMenuEventArgs tabContextMenuEventArgs)
	{
        _mostRecentTabContextMenuEventArgs = tabContextMenuEventArgs;
		
        // The order of 'StateHasChanged(...)' and 'AddActiveDropdownKey(...)' is important.
        // The ChildContent renders nothing, unless the provider of the child content
        // re-renders now that there is a given '_mostRecentTreeViewContextMenuCommandArgs'
        await InvokeAsync(StateHasChanged);

        Dispatcher.Dispatch(new DropdownState.AddActiveAction(
            TabContextMenu.ContextMenuEventDropdownKey));
	}
}