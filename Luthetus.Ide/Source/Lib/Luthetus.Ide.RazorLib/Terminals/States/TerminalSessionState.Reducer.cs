using Fluxor;

namespace Luthetus.Ide.RazorLib.Terminals.States;

public partial record TerminalSessionState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TerminalSessionState ReduceRegisterTerminalSessionAction(
	        TerminalSessionState inState,
	        RegisterTerminalSessionAction registerTerminalSessionAction)
        {
            if (inState.TerminalSessionMap.ContainsKey(registerTerminalSessionAction.TerminalSession.TerminalSessionKey))
                return inState;

            var nextMap = inState.TerminalSessionMap.Add(
                registerTerminalSessionAction.TerminalSession.TerminalSessionKey,
                registerTerminalSessionAction.TerminalSession);

            return inState with { TerminalSessionMap = nextMap };
        }

        [ReducerMethod]
        public static TerminalSessionState ReduceNotifyStateChangedAction(
            TerminalSessionState inState,
            NotifyStateChangedAction notifyStateChangedAction)
        {
            if (!inState.TerminalSessionMap.ContainsKey(notifyStateChangedAction.TerminalSessionKey))
                return inState;

			var inTerminalSession = inState.TerminalSessionMap[notifyStateChangedAction.TerminalSessionKey];

            var nextMap = inState.TerminalSessionMap.SetItem(
				notifyStateChangedAction.TerminalSessionKey,
                inTerminalSession);

            return inState with
            {
                TerminalSessionMap = nextMap
            };
        }

        [ReducerMethod]
        public static TerminalSessionState ReduceDisposeTerminalSessionAction(
            TerminalSessionState inState,
            DisposeTerminalSessionAction disposeTerminalSessionAction)
        {
            var nextMap = inState.TerminalSessionMap.Remove(disposeTerminalSessionAction.TerminalSessionKey);
            return inState with { TerminalSessionMap = nextMap };
        }
    }
}