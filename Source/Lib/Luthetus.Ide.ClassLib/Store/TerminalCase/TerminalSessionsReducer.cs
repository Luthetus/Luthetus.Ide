using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

public class TerminalSessionsReducer
{
    public record RegisterTerminalSessionAction(TerminalSession TerminalSession);
    public record UpdateTerminalSessionStateKeyAction(TerminalSession TerminalSession);
    public record DisposeTerminalSessionAction(TerminalSessionKey TerminalSessionKey);

    [ReducerMethod]
    public static TerminalSessionsState ReduceRegisterTerminalSessionAction(
        TerminalSessionsState inTerminalSessionsState,
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
    public static TerminalSessionsState ReduceUpdateTerminalSessionStateKeyAction(
        TerminalSessionsState inTerminalSessionsState,
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
    public static TerminalSessionsState ReduceDisposeTerminalSessionAction(
        TerminalSessionsState inTerminalSessionsState,
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