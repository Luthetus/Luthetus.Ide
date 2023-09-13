using Fluxor;

namespace Luthetus.Ide.RazorLib.TerminalCase;

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