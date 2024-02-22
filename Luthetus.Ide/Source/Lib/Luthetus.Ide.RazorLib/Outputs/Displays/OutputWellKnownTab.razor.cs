using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Outputs.Displays;

public partial class OutputWellKnownTab : FluxorComponent
{
    [Inject]
    private IState<WellKnownTerminalSessionsState> WellKnownTerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<TerminalSessionState, TerminalSession?> TerminalSessionsStateSelection { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionWasModifiedState> TerminalSessionWasModifiedStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TerminalSession> WellKnownTerminalSessionKey { get; set; } = Key<TerminalSession>.Empty;

    private string CssClassString => $"luth_ide_terminal-tab {ActiveTerminalCommandKeyCssClassString}";

    private string ActiveTerminalCommandKeyCssClassString => IsActiveTerminalCommandKey
        ? "luth_active"
        : TerminalSessionWasModifiedStateWrap.Value.EmptyTextHack;

    private bool IsActiveTerminalCommandKey => WellKnownTerminalSessionKey ==
        WellKnownTerminalSessionsStateWrap.Value.ActiveTerminalSessionKey;

    protected override void OnInitialized()
    {
        TerminalSessionsStateSelection.Select(x =>
        {
            if (x.TerminalSessionMap.TryGetValue(WellKnownTerminalSessionKey, out var wellKnownTerminalSession))
                return wellKnownTerminalSession;

            return null;
        });

        base.OnInitialized();
    }

    private void DispatchSetActiveTerminalCommandKeyActionOnClick()
    {
        Dispatcher.Dispatch(new WellKnownTerminalSessionsState.SetActiveWellKnownTerminalSessionKey(WellKnownTerminalSessionKey));
    }
}