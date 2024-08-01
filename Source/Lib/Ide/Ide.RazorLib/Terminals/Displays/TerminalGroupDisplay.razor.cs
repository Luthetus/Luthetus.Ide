using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.Terminals.Displays;

public partial class TerminalGroupDisplay : ComponentBase
{
    [Inject]
    private IState<TerminalGroupState> TerminalGroupDisplayStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    private void DispatchSetActiveTerminalAction(Key<Terminal> terminalKey)
    {
        Dispatcher.Dispatch(new TerminalGroupState.SetActiveTerminalAction(terminalKey));
    }
    
    private void ClearTerminalOnClick(Key<Terminal> terminalKey)
    {
    	if (terminalKey == TerminalFacts.GENERAL_TERMINAL_KEY)
    		TerminalStateWrap.Value.NEW_TERMINAL?.EnqueueClear();
    }
}