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
        public static TerminalState ReduceNotifyStateChangedAction(
            TerminalState inState,
            NotifyStateChangedAction notifyStateChangedAction)
        {
            if (!inState.TerminalMap.ContainsKey(notifyStateChangedAction.TerminalKey))
                return inState;

			var inTerminal = inState.TerminalMap[notifyStateChangedAction.TerminalKey];

            var nextMap = inState.TerminalMap.SetItem(
				notifyStateChangedAction.TerminalKey,
                inTerminal);

            return inState with
            {
                TerminalMap = nextMap
            };
        }

        [ReducerMethod]
        public static TerminalState ReduceDisposeAction(
            TerminalState inState,
            DisposeAction disposeAction)
        {
            var nextMap = inState.TerminalMap.Remove(disposeAction.TerminalKey);
            return inState with { TerminalMap = nextMap };
        }

        [ReducerMethod]
        public static TerminalState ReduceGeneralTerminalRegisterAction(
            TerminalState inState,
            GeneralTerminalRegisterAction generalTerminalRegisterAction)
        {
            return inState with { GeneralTerminalRegisterAction = generalTerminalRegisterAction.Terminal };
        }

        [ReducerMethod]
        public static TerminalState ReduceExecutionTerminalRegisterAction(
            TerminalState inState,
            ExecutionTerminalRegisterAction executionTerminalRegisterAction)
        {
            return inState with { ExecutionTerminal = executionTerminalRegisterAction.Terminal };
        }
    }
}