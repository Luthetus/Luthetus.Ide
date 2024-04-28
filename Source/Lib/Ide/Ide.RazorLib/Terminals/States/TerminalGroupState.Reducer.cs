using Fluxor;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record class TerminalGroupState
{
    public static class Reducer
    {
        [ReducerMethod]
        public static TerminalGroupState ReduceSetActiveTerminalAction(
            TerminalGroupState inState,
            SetActiveTerminalAction setActiveTerminalAction)
        {
            return inState with
            {
                ActiveTerminalKey = setActiveTerminalAction.TerminalKey
            };
        }
    }
}
