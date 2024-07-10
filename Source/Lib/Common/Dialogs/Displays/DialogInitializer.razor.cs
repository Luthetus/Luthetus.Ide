using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Contexts.Displays;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Displays;

public partial class DialogInitializer : FluxorComponent
{
    [Inject]
    private IState<DialogState> DialogStateWrap { get; set; } = null!;
    
    private ContextBoundary? _dialogContextBoundary;
    
    private Task HandleOnFocusIn(IDialog dialog)
    {
    	var localDialogContextBoundary = _dialogContextBoundary;
    	
    	if (localDialogContextBoundary is not null)
	    	localDialogContextBoundary.HandleOnFocusIn();
    
    	return Task.CompletedTask;
    }
    
    private Task HandleOnFocusOut(IDialog dialog)
    {
    	return Task.CompletedTask;
    }
}