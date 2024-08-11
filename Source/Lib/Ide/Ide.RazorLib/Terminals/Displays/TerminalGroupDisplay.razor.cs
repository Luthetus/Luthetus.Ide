using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

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

    private void DispatchSetActiveTerminalAction(Key<ITerminal> terminalKey)
    {
        Dispatcher.Dispatch(new TerminalGroupState.SetActiveTerminalAction(terminalKey));
    }
    
    private void ClearTerminalOnClick(Key<ITerminal> terminalKey)
    {
    	TerminalStateWrap.Value.TerminalMap[terminalKey]?.EnqueueClear();
    }
}