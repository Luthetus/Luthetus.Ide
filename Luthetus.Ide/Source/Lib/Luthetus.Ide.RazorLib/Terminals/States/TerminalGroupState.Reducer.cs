using Fluxor;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record class TerminalGroupState
{
    public static class Reducer
    {
        [ReducerMethod]
        public static TerminalGroupState ReduceSetActiveTerminalSessionAction(
            TerminalGroupState inState,
            SetActiveTerminalSessionAction setActiveTerminalSessionAction)
        {
            return inState with
            {
                ActiveTerminalSessionKey = setActiveTerminalSessionAction.TerminalSessionKey
            };
        }
    }
}
