using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

public class WellKnownTerminalSessionsStateReducer
{
    [ReducerMethod]
    public static WellKnownTerminalSessionsState ReduceSetActiveTerminalCommandKeyAction(
        WellKnownTerminalSessionsState inWellKnownTerminalSessionsState,
        WellKnownTerminalSessionsState.SetActiveWellKnownTerminalSessionKey setActiveWellKnownTerminalCommandKeyAction)
    {
        return inWellKnownTerminalSessionsState with
        {
            ActiveTerminalSessionKey = setActiveWellKnownTerminalCommandKeyAction
                .TerminalCommandKey
        };
    }
}