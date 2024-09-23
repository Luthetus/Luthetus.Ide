using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.Widgets.States;
using Luthetus.Common.RazorLib.Contexts.Displays;

namespace Luthetus.Common.RazorLib.Widgets.Displays;

public partial class WidgetInitializer : FluxorComponent
{
	[Inject]
    private IState<WidgetState> WidgetStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
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
    	// TODO: neither onfocusout or onblur fit the use case.
    	//       I need to detect when focus leaves either the widget itself
    	//       or leaves its descendents (this part sounds like onfocusout).
    	//       |
    	//       BUT
    	//       |
    	//       I furthermore, ONLY want to have this fire if the newly focused
    	//       HTML element is neither the widget itself or one of its descendents.
    	//       |
    	//       When this event occurs, the widget should no longer render.
    	return Task.CompletedTask;
    }
    
    private Task RemoveWidget()
    {
    	Dispatcher.Dispatch(new WidgetState.SetWidgetAction(null));
    	return Task.CompletedTask;
    }
}