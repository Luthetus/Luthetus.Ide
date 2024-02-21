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
        public static TerminalSessionState ReduceUpdateTerminalSessionStateKeyAction(
            TerminalSessionState inState,
            UpdateTerminalSessionStateKeyAction updateTerminalSessionStateKeyAction)
        {
            if (!inState.TerminalSessionMap.ContainsKey(updateTerminalSessionStateKeyAction.TerminalSession.TerminalSessionKey))
                return inState;

            var nextMap = inState.TerminalSessionMap.SetItem(
                updateTerminalSessionStateKeyAction.TerminalSession.TerminalSessionKey,
                updateTerminalSessionStateKeyAction.TerminalSession);

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