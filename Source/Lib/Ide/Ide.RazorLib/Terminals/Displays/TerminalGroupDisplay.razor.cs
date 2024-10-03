using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Displays.Internals;

namespace Luthetus.Ide.RazorLib.Terminals.Displays;

public partial class TerminalGroupDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalGroupState> TerminalGroupDisplayStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

	private Key<IDynamicViewModel> _addIntegratedTerminalDialogKey = Key<IDynamicViewModel>.NewKey();

    private void DispatchSetActiveTerminalAction(Key<ITerminal> terminalKey)
    {
        Dispatcher.Dispatch(new TerminalGroupState.SetActiveTerminalAction(terminalKey));
    }
    
    private void ClearTerminalOnClick(Key<ITerminal> terminalKey)
    {
    	TerminalStateWrap.Value.TerminalMap[terminalKey]?.ClearFireAndForget();
    }
    
    private void AddIntegratedTerminalOnClick()
    {
    	var addIntegratedTerminalDialog = new DialogViewModel(
            _addIntegratedTerminalDialogKey,
			"Add Integrated Terminal",
            typeof(AddIntegratedTerminalDisplay),
            componentParameterMap: null,
            cssClass: null,
			isResizable: true,
			setFocusOnCloseElementId: null);

        Dispatcher.Dispatch(new DialogState.RegisterAction(addIntegratedTerminalDialog));
    }
}