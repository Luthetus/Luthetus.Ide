using Fluxor;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record WellKnownTerminalState
{
    public class Reducer
    {
        [ReducerMethod]
        public static WellKnownTerminalState ReduceSetActiveTerminalCommandKeyAction(
            WellKnownTerminalState inState,
            SetActiveWellKnownTerminalKey setActiveWellKnownTerminalKeyAction)
        {
            return inState with
            {
                ActiveTerminalKey = setActiveWellKnownTerminalKeyAction.TerminalKey
            };
        }
    }
}