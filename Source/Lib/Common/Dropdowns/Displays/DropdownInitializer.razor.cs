using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Contexts.Displays;

namespace Luthetus.Common.RazorLib.Dropdowns.Displays;

public partial class DropdownInitializer : FluxorComponent
{
	[Inject]
    private IState<DropdownState> DropdownStateWrap { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

	private ContextBoundary? _dropdownContextBoundary;

    private async Task ClearActiveKeyList()
    {
    	var firstDropdown = DropdownStateWrap.Value.DropdownList.FirstOrDefault();
    	
    	if (firstDropdown is not null)
    	{
    		var restoreFocusOnCloseFunc = firstDropdown.RestoreFocusOnClose;
    		
    		if (restoreFocusOnCloseFunc is not null)
    			await restoreFocusOnCloseFunc.Invoke();
    	}
    	
        Dispatcher.Dispatch(new DropdownState.ClearAction());
    }
    
    private Task HandleOnFocusIn(DropdownRecord dropdown)
    {
    	var localDropdownContextBoundary = _dropdownContextBoundary;
    	
    	if (localDropdownContextBoundary is not null)
	    	localDropdownContextBoundary.HandleOnFocusIn();
    
    	return Task.CompletedTask;
    }
    
    private Task HandleOnFocusOut(DropdownRecord dropdown)
    {
    	return Task.CompletedTask;
    }
}