using Fluxor;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record TerminalState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TerminalState ReduceRegisterAction(
	        TerminalState inState,
	        RegisterAction registerAction)
        {
            if (inState.TerminalMap.ContainsKey(registerAction.Terminal.Key))
                return inState;

            var nextMap = inState.TerminalMap.Add(
                registerAction.Terminal.Key,
                registerAction.Terminal);

            return inState with { TerminalMap = nextMap };
        }

        [ReducerMethod]
        public static TerminalState ReduceStateHasChangedAction(
            TerminalState inState,
            StateHasChangedAction stateHasChangedAction)
        {
            return inState with {};
        }

        [ReducerMethod]
        public static TerminalState ReduceDisposeAction(
            TerminalState inState,
            DisposeAction disposeAction)
        {
            var nextMap = inState.TerminalMap.Remove(disposeAction.TerminalKey);
            return inState with { TerminalMap = nextMap };
        }
    }
}