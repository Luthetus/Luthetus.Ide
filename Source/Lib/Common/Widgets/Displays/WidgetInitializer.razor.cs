using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.Widgets.States;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Widgets.Displays;

public partial class WidgetInitializer : FluxorComponent
{
	[Inject]
    private IState<WidgetState> WidgetStateWrap { get; set; } = null!;
    
    private ContextBoundary? _widgetContextBoundary;
    
    private Task HandleOnFocusIn(WidgetModel widget)
    {
    	var localWidgetContextBoundary = _widgetContextBoundary;
    	
    	if (localWidgetContextBoundary is not null)
	    	localWidgetContextBoundary.HandleOnFocusIn();
    
    	return Task.CompletedTask;
    }
    
    private Task HandleOnFocusOut(WidgetModel widget)
    {
    	return Task.CompletedTask;
    }
    
    private Task RemoveWidget()
    {
    	return Task.CompletedTask;
    }
}