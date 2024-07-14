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
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.ProgramExecutions.States;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.StartupControls.Displays;

public partial class StartupControlsDisplay : FluxorComponent
{
    [Inject]
    private IState<ProgramExecutionState> ProgramExecutionStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelState> PanelStateWrap { get; set; } = null!;
    [Inject]
    private DotNetCliOutputParser DotNetCliOutputParser { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private const string _startButtonElementId = "luth_ide_startup-controls-display_id";

    private readonly Key<TerminalCommand> _newDotNetSolutionTerminalCommandKey = Key<TerminalCommand>.NewKey();
    private readonly CancellationTokenSource _newDotNetSolutionCancellationTokenSource = new();
    
    private TerminalCommand? _executingTerminalCommand;
    
    private ElementReference? _startButtonElementReference;
    private Key<DropdownRecord> _startButtonDropdownKey = Key<DropdownRecord>.NewKey();
    
    private LuthetusCommonJavaScriptInteropApi? _jsRuntimeCommonApi;
    
    private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi =>
    	_jsRuntimeCommonApi ??= JsRuntime.GetLuthetusCommonApi();

    private TerminalCommand? GetStartProgramTerminalCommand()
    {
        var programExecutionState = ProgramExecutionStateWrap.Value;

        if (programExecutionState.StartupProjectAbsolutePath is null)
            return null;

        var ancestorDirectory = programExecutionState.StartupProjectAbsolutePath.ParentDirectory;

        if (ancestorDirectory is null)
            return null;

        var formattedCommand = DotNetCliCommandFormatter.FormatStartProjectWithoutDebugging(
            programExecutionState.StartupProjectAbsolutePath);

        return new TerminalCommand(
            _newDotNetSolutionTerminalCommandKey,
            formattedCommand,
            ancestorDirectory.Value,
            _newDotNetSolutionCancellationTokenSource.Token,
            OutputParser: DotNetCliOutputParser);
    }

    private async Task StartProgramWithoutDebuggingOnClick(bool isExecuting)
    {
    	if (isExecuting)
    	{
    		await RenderDropdownOnClick();
		}
        else
        {
	        var startProgramTerminalCommand = GetStartProgramTerminalCommand();
	        if (startProgramTerminalCommand is null)
	            return;
	
			_executingTerminalCommand = startProgramTerminalCommand;
	        var executionTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_TERMINAL_KEY];
	        executionTerminal.EnqueueCommand(startProgramTerminalCommand);
        }
    }
    
    private async Task RenderDropdownOnClick()
	{
		var buttonDimensions = await JsRuntimeCommonApi
			.MeasureElementById(_startButtonElementId)
			.ConfigureAwait(false);
			
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
			}));
		    
		menuOptionList.Add(new MenuOptionRecord(
			"Stop Execution",
		    MenuOptionKind.Other,
		    OnClickFunc: () =>
		    {
		    	var executionTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_TERMINAL_KEY];
		    	executionTerminal.KillProcess();
                return Task.CompletedTask;
		    }));

		var dropdownRecord = new DropdownRecord(
			_startButtonDropdownKey,
			buttonDimensions.LeftInPixels,
			buttonDimensions.TopInPixels + buttonDimensions.HeightInPixels,
			typeof(MenuDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(MenuDisplay.MenuRecord),
					new MenuRecord(menuOptionList.ToImmutableArray())
				}
			},
			() => RestoreFocusToElementReference(_startButtonElementReference));

        Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
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