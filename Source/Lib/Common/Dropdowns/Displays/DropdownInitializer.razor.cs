using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Contexts.Displays;

namespace Luthetus.Common.RazorLib.Dropdowns.Displays;

public partial class DropdownInitializer : ComponentBase, IDisposable
{
	[Inject]
    private IDropdownService DropdownService { get; set; } = null!;

	private ContextBoundary? _dropdownContextBoundary;
	
	protected override void OnInitialized()
	{
		DropdownService.DropdownStateChanged += OnDropdownStateChanged;
		base.OnInitialized();
	}

    private async Task ClearActiveKeyList()
    {
    	var firstDropdown = DropdownService.GetDropdownState().DropdownList.FirstOrDefault();
    	
    	if (firstDropdown is not null)
    	{
    		var restoreFocusOnCloseFunc = firstDropdown.RestoreFocusOnClose;
    		
    		if (restoreFocusOnCloseFunc is not null)
    			await restoreFocusOnCloseFunc.Invoke();
    	}
    	
        DropdownService.ReduceClearAction();
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
    
    public async void OnDropdownStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	DropdownService.DropdownStateChanged -= OnDropdownStateChanged;
    }
}