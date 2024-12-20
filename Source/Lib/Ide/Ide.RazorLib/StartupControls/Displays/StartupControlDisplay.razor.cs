using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.StartupControls.States;
using Luthetus.Ide.RazorLib.StartupControls.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.Displays;

public partial class StartupControlDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelState> PanelStateWrap { get; set; } = null!;
    [Inject]
    private IState<StartupControlState> StartupControlStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private const string _startButtonElementId = "luth_ide_startup-controls-display_id";

    private ElementReference? _startButtonElementReference;
    private Key<DropdownRecord> _startButtonDropdownKey = Key<DropdownRecord>.NewKey();
    
    public string? SelectedStartupControlGuidString
    {
    	get => StartupControlStateWrap.Value.ActiveStartupControlKey.Guid.ToString();
    	set
    	{
    		Key<IStartupControlModel> startupControlKey = Key<IStartupControlModel>.Empty;
    		
    		if (value is not null &&
    			Guid.TryParse(value, out var guid))
    		{
    			startupControlKey = new Key<IStartupControlModel>(guid);
    		}
    		
    		Dispatcher.Dispatch(new StartupControlState.SetActiveStartupControlKeyAction(startupControlKey));
    	}
    }
    
    private LuthetusCommonJavaScriptInteropApi? _jsRuntimeCommonApi;
    
    private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi =>
    	_jsRuntimeCommonApi ??= JsRuntime.GetLuthetusCommonApi();

    private async Task StartProgramWithoutDebuggingOnClick(bool isExecuting)
    {
    	var localStartupControlState = StartupControlStateWrap.Value;
    	
    	if (localStartupControlState.ActiveStartupControl is null)
	    	return;
    
    	if (isExecuting)
    	{
    		var menuOptionList = new List<MenuOptionRecord>();
			
			menuOptionList.Add(new MenuOptionRecord(
				"View Output",
			    MenuOptionKind.Other,
			    OnClickFunc: async () => 
				{
					var success = await TrySetFocus(ContextFacts.OutputContext).ConfigureAwait(false);
	
	                if (!success)
	                {
	                    Dispatcher.Dispatch(new PanelState.SetPanelTabAsActiveByContextRecordKeyAction(
	                        ContextFacts.OutputContext.ContextKey));
	
	                    _ = await TrySetFocus(ContextFacts.OutputContext).ConfigureAwait(false);
	                }
				}));
			    
			menuOptionList.Add(new MenuOptionRecord(
				"View Terminal",
			    MenuOptionKind.Other,
			    OnClickFunc: async () => 
				{
					var success = await TrySetFocus(ContextFacts.TerminalContext).ConfigureAwait(false);
	
	                if (!success)
	                {
	                    Dispatcher.Dispatch(new PanelState.SetPanelTabAsActiveByContextRecordKeyAction(
	                        ContextFacts.TerminalContext.ContextKey));
	
	                    _ = await TrySetFocus(ContextFacts.TerminalContext).ConfigureAwait(false);
	                }
	                
	                Dispatcher.Dispatch(new TerminalGroupState.SetActiveTerminalAction(TerminalFacts.EXECUTION_KEY));
				}));
			    
			menuOptionList.Add(new MenuOptionRecord(
				"Stop Execution",
			    MenuOptionKind.Other,
			    OnClickFunc: () =>
			    {
			    	var localStartupControlState = StartupControlStateWrap.Value;
			    	
			    	if (localStartupControlState.ActiveStartupControl is null)
			    		return Task.CompletedTask;
			    		
			    	return localStartupControlState.ActiveStartupControl.StopButtonOnClickTask
			    		.Invoke(localStartupControlState.ActiveStartupControl);
			    }));
			    
			await DropdownHelper.RenderDropdownAsync(
    			Dispatcher,
    			JsRuntimeCommonApi,
				_startButtonElementId,
				DropdownOrientation.Bottom,
				_startButtonDropdownKey,
				new MenuRecord(menuOptionList.ToImmutableArray()),
				_startButtonElementReference);
		}
        else
        {
	        await localStartupControlState.ActiveStartupControl.StartButtonOnClickTask
	        	.Invoke(localStartupControlState.ActiveStartupControl)
	        	.ConfigureAwait(false);
        }
    }
	
	private async Task<bool> TrySetFocus(ContextRecord contextRecord)
    {
        return await JsRuntimeCommonApi
            .TryFocusHtmlElementById(contextRecord.ContextElementId)
            .ConfigureAwait(false);
    }
	
	private async Task RestoreFocusToElementReference(ElementReference? elementReference)
    {
        try
        {
            if (elementReference is not null)
            {
                await elementReference.Value
                    .FocusAsync()
                    .ConfigureAwait(false);
            }
        }
        catch (Exception)
        {
			// TODO: Capture specifically the exception that is fired when the JsRuntime...
			//       ...tries to set focus to an HTML element, but that HTML element
			//       was not found.
        }
    }
}