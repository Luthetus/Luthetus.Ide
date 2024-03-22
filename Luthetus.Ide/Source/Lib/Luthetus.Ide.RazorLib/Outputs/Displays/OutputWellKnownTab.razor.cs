using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;

namespace Luthetus.Ide.RazorLib.Outputs.Displays;

public partial class OutputWellKnownTab : FluxorComponent
{
    [Inject]
    private IState<WellKnownTerminalSessionsState> WellKnownTerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<TerminalSessionState, TerminalSession?> TerminalSessionStateSelection { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TerminalSession> WellKnownTerminalSessionKey { get; set; } = Key<TerminalSession>.Empty;

    private string CssClassString => $"luth_ide_terminal-tab {ActiveTerminalCommandKeyCssClassString}";

    private string ActiveTerminalCommandKeyCssClassString => IsActiveTerminalCommandKey
        ? "luth_active"
        : string.Empty;

    private bool IsActiveTerminalCommandKey => WellKnownTerminalSessionKey ==
		WellKnownTerminalSessionsStateWrap.Value.ActiveTerminalSessionKey;

    protected override void OnInitialized()
    {
        TerminalSessionStateSelection.Select(x =>
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