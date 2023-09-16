using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.KeyCase;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TerminalCase.Displays;

public partial class TerminalWellKnownTab : FluxorComponent
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

    private string CssClassString =>
        $"luth_ide_terminal-tab {ActiveTerminalCommandKeyCssClassString}";

    private string ActiveTerminalCommandKeyCssClassString =>
        IsActiveTerminalCommandKey
            ? "luth_active"
            : TerminalSessionWasModifiedStateWrap.Value.EmptyTextHack;

    private bool IsActiveTerminalCommandKey =>
        WellKnownTerminalSessionsStateWrap.Value.ActiveTerminalSessionKey ==
        WellKnownTerminalSessionKey;

    protected override void OnInitialized()
    {
        TerminalSessionsStateSelection
            .Select(x =>
            {
                if (x.TerminalSessionMap.TryGetValue(
                        WellKnownTerminalSessionKey, out var wellKnownTerminalSession))
                {
                    return wellKnownTerminalSession;
                }

                return null;
            });

        base.OnInitialized();
    }

    private Task DispatchSetActiveTerminalCommandKeyActionOnClick()
    {
        Dispatcher.Dispatch(new WellKnownTerminalSessionsState.SetActiveWellKnownTerminalSessionKey(
            WellKnownTerminalSessionKey));

        return Task.CompletedTask;
    }

    private Task ClearStandardOutOnClick()
    {
        TerminalSessionsStateSelection.Value
            .ClearStandardOut();

        return Task.CompletedTask;
    }

    private Task KillProcessOnClick()
    {
        TerminalSessionsStateSelection.Value
            .KillProcess();

        return Task.CompletedTask;
    }
}