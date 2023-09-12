using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

public class TerminalSessionRegistryReducer
{
    public record RegisterTerminalSessionAction(TerminalSession TerminalSession);
    public record UpdateTerminalSessionStateKeyAction(TerminalSession TerminalSession);
    public record DisposeTerminalSessionAction(TerminalSessionKey TerminalSessionKey);

    [ReducerMethod]
    public static TerminalSessionRegistry ReduceRegisterTerminalSessionAction(
        TerminalSessionRegistry inTerminalSessionsState,
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
    public static TerminalSessionRegistry ReduceUpdateTerminalSessionStateKeyAction(
        TerminalSessionRegistry inTerminalSessionsState,
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
    public static TerminalSessionRegistry ReduceDisposeTerminalSessionAction(
        TerminalSessionRegistry inTerminalSessionsState,
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