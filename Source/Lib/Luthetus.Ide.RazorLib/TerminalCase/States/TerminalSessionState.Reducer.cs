using Fluxor;
using Luthetus.Ide.RazorLib.TerminalCase.Models;

namespace Luthetus.Ide.RazorLib.TerminalCase.States;

public partial record TerminalSessionState
{
    private class Reducer
    {
        [ReducerMethod]
        public static TerminalSessionState ReduceRegisterTerminalSessionAction(
        TerminalSessionState inTerminalSessionsState,
        RegisterTerminalSessionAction registerTerminalSessionAction)
        {
            if (inTerminalSessionsState.TerminalSessionMap.ContainsKey(
                    registerTerminalSessionAction.TerminalSession.TerminalSessionKey))
            {
                return inTerminalSessionsState;
            }

            var nextMap = inTerminalSessionsState.TerminalSessionMap
                .Add(
                    registerTerminalSessionAction.TerminalSession.TerminalSessionKey,
                    registerTerminalSessionAction.TerminalSession);

            return inTerminalSessionsState with
            {
                TerminalSessionMap = nextMap
            };
        }

        [ReducerMethod]
        public static TerminalSessionState ReduceUpdateTerminalSessionStateKeyAction(
            TerminalSessionState inTerminalSessionsState,
            UpdateTerminalSessionStateKeyAction updateTerminalSessionStateKeyAction)
        {
            if (inTerminalSessionsState.TerminalSessionMap.ContainsKey(
                    updateTerminalSessionStateKeyAction.TerminalSession.TerminalSessionKey))
            {
                var nextMap = inTerminalSessionsState.TerminalSessionMap
                    .SetItem(
                        updateTerminalSessionStateKeyAction.TerminalSession.TerminalSessionKey,
                        updateTerminalSessionStateKeyAction.TerminalSession);

                return inTerminalSessionsState with
                {
                    TerminalSessionMap = nextMap
                };
            }
            else
            {
                return inTerminalSessionsState;
            }
        }

        [ReducerMethod]
        public static TerminalSessionState ReduceDisposeTerminalSessionAction(
            TerminalSessionState inTerminalSessionsState,
            DisposeTerminalSessionAction disposeTerminalSessionAction)
        {
            var nextMap = inTerminalSessionsState.TerminalSessionMap
                .Remove(disposeTerminalSessionAction.TerminalSessionKey);

            return inTerminalSessionsState with
            {
                TerminalSessionMap = nextMap
            };
        }
    }
}