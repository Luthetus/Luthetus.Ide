using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Ide.RazorLib.CommandBars.States;

namespace Luthetus.Ide.RazorLib.CommandBars.Displays;

public partial class CommandBarDisplay : FluxorComponent
{
	[Inject]
	private IState<CommandBarState> CommandBarStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
	private IJSRuntime JsRuntime { get; set; } = null!;
	
	public const string INPUT_HTML_ELEMENT_ID = "luth_ide_command-bar-input-id";
	
	private bool _previousShouldDisplay = false;
	private LuthetusCommonJavaScriptInteropApi _jsRuntimeCommonApi;
	
	private LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi => _jsRuntimeCommonApi
		??= JsRuntime.GetLuthetusCommonApi();
	
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		var shouldDisplay = CommandBarStateWrap.Value.ShouldDisplay;
		
		if (_previousShouldDisplay != shouldDisplay)
		{
			_previousShouldDisplay = shouldDisplay;
			
			if (shouldDisplay)
			{
				await JsRuntimeCommonApi
					.FocusHtmlElementById(CommandBarDisplay.INPUT_HTML_ELEMENT_ID)
	                .ConfigureAwait(false);
			}
		}
        			
		await base.OnAfterRenderAsync(firstRender);
	}
	
	private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
	{
		if (keyboardEventArgs.Key == "Enter")
		{
			Dispatcher.Dispatch(new CommandBarState.SetShouldDisplayAction(false));
		}
		else if (keyboardEventArgs.Key == "Escape")
		{
			Dispatcher.Dispatch(new CommandBarState.SetShouldDisplayAction(false));
		}
	}
	
	private Task OnOutOfBoundsClick()
	{
		Dispatcher.Dispatch(new CommandBarState.SetShouldDisplayAction(false));
		return Task.CompletedTask;
	}
}