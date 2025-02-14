using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Dropdowns.Displays;

public partial class DropdownInitializer : ComponentBase, IDisposable
{
	[Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

	private ContextBoundary? _dropdownContextBoundary;
	
	protected override void OnInitialized()
	{
		CommonApi.DropdownApi.DropdownStateChanged += OnDropdownStateChanged;
		base.OnInitialized();
	}

    private async Task ClearActiveKeyList()
    {
    	var firstDropdown = CommonApi.DropdownApi.GetDropdownState().DropdownList.FirstOrDefault();
    	
    	if (firstDropdown is not null)
    	{
    		var restoreFocusOnCloseFunc = firstDropdown.RestoreFocusOnClose;
    		
    		if (restoreFocusOnCloseFunc is not null)
    			await restoreFocusOnCloseFunc.Invoke();
    	}
    	
        CommonApi.DropdownApi.ReduceClearAction();
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
    	CommonApi.DropdownApi.DropdownStateChanged -= OnDropdownStateChanged;
    }
}