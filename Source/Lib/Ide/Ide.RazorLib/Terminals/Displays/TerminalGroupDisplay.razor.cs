using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Terminals.Displays;

public partial class TerminalGroupDisplay : ComponentBase
{
    [Inject]
    private IState<TerminalGroupState> TerminalGroupDisplayStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private void DispatchSetActiveTerminalAction(Key<Terminal> terminalKey)
    {
        Dispatcher.Dispatch(new TerminalGroupState.SetActiveTerminalAction(terminalKey));
    }
}