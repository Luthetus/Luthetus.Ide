using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.Displays;

public partial class StartupControlDisplay : ComponentBase, IDisposable
{
    [Inject]
    private ITerminalGroupService TerminalGroupService { get; set; } = null!;
    [Inject]
    private ITerminalService TerminalService { get; set; } = null!;
    [Inject]
    private IStartupControlService StartupControlService { get; set; } = null!;
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private const string _startButtonElementId = "luth_ide_startup-controls-display_id";

    private ElementReference? _startButtonElementReference;
    private Key<DropdownRecord> _startButtonDropdownKey = Key<DropdownRecord>.NewKey();
    
    public string? SelectedStartupControlGuidString
    {
    	get => StartupControlService.GetStartupControlState().ActiveStartupControlKey.Guid.ToString();
    	set
    	{
    		Key<IStartupControlModel> startupControlKey = Key<IStartupControlModel>.Empty;
    		
    		if (value is not null &&
    			Guid.TryParse(value, out var guid))
    		{
    			startupControlKey = new Key<IStartupControlModel>(guid);
    		}
    		
    		StartupControlService.ReduceSetActiveStartupControlKeyAction(startupControlKey);
    	}
    }
    
    private LuthetusCommonJavaScriptInteropApi? _jsRuntimeCommonApi;
    
    private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi =>
    	_jsRuntimeCommonApi ??= JsRuntime.GetLuthetusCommonApi();
    	
    protected override void OnInitialized()
    {
    	TerminalService.TerminalStateChanged += OnTerminalStateChanged;
    	StartupControlService.StartupControlStateChanged += OnStartupControlStateChanged;
    	base.OnInitialized();
    }

    private async Task StartProgramWithoutDebuggingOnClick(bool isExecuting)
    {
    	var localStartupControlState = StartupControlService.GetStartupControlState();
    	
    	if (localStartupControlState.ActiveStartupControl is null)
	    	return;
    
    	if (isExecuting)
    	{
    		var menuOptionList = new List<MenuOptionRecord>();
			
			menuOptionList.Add(new MenuOptionRecord(
				"View Output",
			    MenuOptionKind.Other,
			    onClickFunc: async () => 
				{
					var success = await TrySetFocus(ContextFacts.OutputContext).ConfigureAwait(false);
	
	                if (!success)
	                {
	                    PanelService.ReduceSetPanelTabAsActiveByContextRecordKeyAction(
	                        ContextFacts.OutputContext.ContextKey);
	
	                    _ = await TrySetFocus(ContextFacts.OutputContext).ConfigureAwait(false);
	                }
				}));
			    
			menuOptionList.Add(new MenuOptionRecord(
				"View Terminal",
			    MenuOptionKind.Other,
			    onClickFunc: async () => 
				{
					var success = await TrySetFocus(ContextFacts.TerminalContext).ConfigureAwait(false);
	
	                if (!success)
	                {
	                    PanelService.ReduceSetPanelTabAsActiveByContextRecordKeyAction(
	                        ContextFacts.TerminalContext.ContextKey);
	
	                    _ = await TrySetFocus(ContextFacts.TerminalContext).ConfigureAwait(false);
	                }
	                
	                TerminalGroupService.ReduceSetActiveTerminalAction(TerminalFacts.EXECUTION_KEY);
				}));
			    
			menuOptionList.Add(new MenuOptionRecord(
				"Stop Execution",
			    MenuOptionKind.Other,
			    onClickFunc: () =>
			    {
			    	var localStartupControlState = StartupControlService.GetStartupControlState();
			    	
			    	if (localStartupControlState.ActiveStartupControl is null)
			    		return Task.CompletedTask;
			    		
			    	return localStartupControlState.ActiveStartupControl.StopButtonOnClickTask
			    		.Invoke(localStartupControlState.ActiveStartupControl);
			    }));
			    
			await DropdownHelper.RenderDropdownAsync(
    			DropdownService,
    			JsRuntimeCommonApi,
				_startButtonElementId,
				DropdownOrientation.Bottom,
				_startButtonDropdownKey,
				new MenuRecord(menuOptionList),
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
    
    private async void OnTerminalStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    private async void OnStartupControlStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	TerminalService.TerminalStateChanged -= OnTerminalStateChanged;
    	StartupControlService.StartupControlStateChanged -= OnStartupControlStateChanged;
    }
}