using Fluxor;

namespace Luthetus.Ide.Tests.Basis.Terminals.States;

public partial record WellKnownTerminalSessionsStateReducerTests
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