using Fluxor;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record WellKnownTerminalSessionsState
{
    private class Reducer
    {
        [ReducerMethod]
        public static WellKnownTerminalSessionsState ReduceSetActiveTerminalCommandKeyAction(
            WellKnownTerminalSessionsState inState,
            SetActiveWellKnownTerminalSessionKey setActiveWellKnownTerminalSessionKeyAction)
        {
            return inState with
            {
                ActiveTerminalSessionKey = setActiveWellKnownTerminalSessionKeyAction.TerminalSessionKey
            };
        }
    }
}