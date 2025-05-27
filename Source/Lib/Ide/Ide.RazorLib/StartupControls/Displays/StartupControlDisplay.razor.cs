using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
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
    private IPanelService PanelService { get; set; } = null!;
    [Inject]
    private IDropdownService DropdownService { get; set; } = null!;
    [Inject]
    private IStartupControlService StartupControlService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; } = null!;

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
    		
    		StartupControlService.SetActiveStartupControlKey(startupControlKey);
    	}
    }
    	
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
	                    PanelService.SetPanelTabAsActiveByContextRecordKey(
	                        ContextFacts.OutputContext.ContextKey);
	
	                    _ = await TrySetFocus(ContextFacts.OutputContext).ConfigureAwait(false);
	                }
				}));
			    
			menuOptionList.Add(new MenuOptionRecord(
				"View Terminal",
			    MenuOptionKind.Other,
			    onClickFunc: async () => 
				{
					TerminalGroupService.SetActiveTerminal(TerminalFacts.EXECUTION_KEY);
				
					var success = await TrySetFocus(ContextFacts.TerminalContext).ConfigureAwait(false);
	
	                if (!success)
	                {
	                    PanelService.SetPanelTabAsActiveByContextRecordKey(
	                        ContextFacts.TerminalContext.ContextKey);
	
	                    _ = await TrySetFocus(ContextFacts.TerminalContext).ConfigureAwait(false);
	                }
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
    			CommonBackgroundTaskApi.JsRuntimeCommonApi,
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
        return await CommonBackgroundTaskApi.JsRuntimeCommonApi
            .TryFocusHtmlElementById(contextRecord.ContextElementId)
            .ConfigureAwait(false);
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