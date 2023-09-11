using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

public class WellKnownTerminalSessionsRegistryReducer
{
    [ReducerMethod]
    public static WellKnownTerminalSessionsRegistry ReduceSetActiveTerminalCommandKeyAction(
        WellKnownTerminalSessionsRegistry inWellKnownTerminalSessionsState,
        WellKnownTerminalSessionsRegistry.SetActiveWellKnownTerminalSessionKey setActiveWellKnownTerminalCommandKeyAction)
    {
        return inWellKnownTerminalSessionsState with
        {
            ActiveTerminalSessionKey = setActiveWellKnownTerminalCommandKeyAction
                .TerminalCommandKey
        };
    }
}