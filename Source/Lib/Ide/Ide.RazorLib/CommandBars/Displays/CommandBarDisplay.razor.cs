using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.CommandBars.States;

namespace Luthetus.Ide.RazorLib.CommandBars.Displays;

public partial class CommandBarDisplay : FluxorComponent
{
	[Inject]
	private IState<CommandBarState> CommandBarStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
	
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
}